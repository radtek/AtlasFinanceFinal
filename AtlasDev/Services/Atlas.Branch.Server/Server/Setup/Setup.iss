;
; Copyright(C) 2013-2016 - Atlas Finance (Pty) Ltd.
;
; -------------------------------------------------------------
; Written by Keith Blows
;
; Compile this with Inno-setup / Inno-downloader
;
;
; Revision history
; ------------------------------------------------------------- 
;   20 August 2013- Created
;
;   18 Sept 2013- Updated
;   
;   19 Feb 2014 - Added wyUpdate files
;
;    1 Oct 2014 - Updated for new locations
;
;
#define MyAppName "ASS Branch Data Sync Server"
#define MyAppVersion "Version 1.0"
#define MyAppPublisher "Atlas Finance (Pty) Ltd."
#define MyAppURL "http://www.atlasfinance.co.za"
#define MyAppExeName "ASSSyncClient.exe"
#define MyAppInstallPath "c:\Atlas\LMS\DataSync"

#define Version "1.2.0.3"
#define SourceAppPath     "D:\AtlasDev\Services\Atlas.Branch.Server\wyUpdate\Release\V" + Version
#define SourcewyBuildPath "D:\AtlasDev\Services\Atlas.Branch.Server\wyUpdate\wyUpdate\"

#define SourceConfig      "D:\AtlasDev\Services\Atlas.Branch.Server\Server\App.release.config"
#define SourceAppSettings "D:\AtlasDev\Services\Atlas.Branch.Server\Server\appSettings.config"


[Setup]
AppID={{AF891199-BC88-48E5-9EE3-ADA412EBDD14}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={#MyAppInstallPath}
DisableDirPage=yes
DefaultGroupName=Atlas\DataSync
DisableProgramGroupPage=yes
OutputBaseFilename=DataSyncSetup
SetupIconFile=D:\AtlasDev\Resources\Icons\AtlasLib.ico
Compression=lzma2/Max
SolidCompression=true
AppCopyright=Atlas Finance (Pty) Ltd.
AppMutex=Atlas DataCopy
PrivilegesRequired=admin
ShowLanguageDialog=no
VersionInfoVersion={#Version}
VersionInfoCompany=Atlas Finance (Pty) Ltd.
VersionInfoCopyright=Atlas Finance (Pty) Ltd.
UninstallDisplayIcon={app}\ASSSyncClient.exe
UninstallDisplayName=Atlas Data Sync Server- Uninstall
MinVersion=0,5.1.2600sp1

SignTool=CodeSign $p


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Dirs]
Name: "c:\Atlas\LMS\Updates\Ass"

[Files]
Source: "{#SourceConfig}"; DestDir: "{app}"; DestName: "ASSSyncClient.exe.config";
Source: "{#SourceAppSettings}"; DestDir: "{app}"; DestName: "appSettings.config"; AfterInstall: ConfigureConfig

; Core
Source: "{#SourceAppPath}\ASSSyncClient.exe"; DestDir: "{app}"

; DLL's
Source: "{#SourceAppPath}\*.dll"; DestDir: "{app}"

; wyUpdate
Source: "{#SourcewyBuildPath}\wyUpdate.exe"; DestDir: "{app}"
Source: "{#SourcewyBuildPath}\Client.wyc"; DestDir: "{app}"
Source: "C:\Program Files (x86)\wyBuild\AutomaticUpdater\Microsoft .NET 4.0\AutomaticUpdater.dll"; DestDir: "{app}"

; ASS wyUpdate
Source: "D:\AtlasDev\Desktop\ASS- wyupdate\wyUpdate\wyUpdate.exe"; DestDir: "c:\Atlas\LMS\Updates\Ass"; Flags: onlyifdoesntexist
Source: "D:\AtlasDev\Desktop\ASS- wyupdate\wyUpdate\client.wyc"; DestDir: "c:\Atlas\LMS\Updates\Ass"; Flags: onlyifdoesntexist

[Run]
Filename: "{app}\{#MyAppExeName}"; Parameters: "install"; WorkingDir: "{app}"; Description: "Install service"; StatusMsg: "Installing the Atlas ASS Data Sync Service... please wait"
Filename: "{app}\{#MyAppExeName}"; Parameters: "start"; WorkingDir: "{app}"; Description: "Starting service"; StatusMsg: "Starting the Atlas ASS Data Sync Service... please wait"

[UninstallRun]
Filename: "{app}\{#MyAppExeName}"; Parameters: "stop"; WorkingDir: "{app}"
Filename: "{app}\{#MyAppExeName}"; Parameters: "uninstall"; WorkingDir: "{app}"


[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[Code]
var
  ProgressPage: TOutputProgressWizardPage;   // Custom dialog to show .NET installation in progress
  InputPage : TInputQueryWizardPage;
      
const
  APP_DIR = 'C:\Atlas\Client\Setup\';

  InputQueryPageID = 101;  // First created form is 100, second is 101...


// Check .NET exists and force user to download the necessary MS setup files 
procedure InitializeWizard();
var
  lResultCode: Integer;

begin   
  // Create our custom branch code input form
  InputPage := CreateInputQueryPage(wpWelcome, 'User input required', 
    'Atlas 2-digit branch code ', 
    'Please enter this machine''s 2-digit branch code for this server (i.e. 01 or C1)');
  InputPage.Add('Branch code:', False);

  // Locate to download/install- must exist for downloading/checking downloads exist
  ForceDirectories(APP_DIR);
  
  // On installation- ensure the Atlas Core service is not running
  if (CheckForMutexes('Global\Atlas Core Service V1')) then
  begin
    Exec(ExpandConstant('{sys}\sc.exe'), 'STOP AtlasCoreClient.V1.0', '', SW_HIDE, ewWaitUntilTerminated, lResultCode);
  end;

end;



/// Used to check user input
function NextButtonClick(CurrPageID: Integer) : Boolean;
var
  lBranchNum: String;
  lDigit1: Char;
  lDigit2: Char;

begin
  Result := True;
    
  if (CurrPageID = InputQueryPageID) then
  begin         
    lBranchNum := InputPage.Values[0];
    if (Length(lBranchNum) = 2) then
    begin
      lDigit1 := lBranchNum[1];
      lDigit2 := lBranchNum[2];
    end else
    begin
      lDigit1 := Chr(0);
      lDigit2 := Chr(0);      
    end;      

    if ((Length(lBranchNum) <> 2) or 
        (lDigit1 > 'Z') or (lDigit1 < '0') or 
        (lDigit2 > '9') or (lDigit2 < '0')) then  // or (not lBranchNum[1] in ['0'..'9']) ) then
    begin
      MsgBox('Invalid branch number', mbInformation, MB_OK);
      Result := False;
    end;
  end;
end;


procedure ConfigureConfig();
var
  lConfig: TStringList;
  i: Integer;
  
begin  
  lConfig := TStringList.Create();
  try
    lConfig.LoadFromFile(ExpandConstant('{app}\appSettings.config'));    
    
    for i:= 0 to lConfig.Count - 1 do
    begin
      if (Pos('legacybranchnum', Lowercase(lConfig[i])) > 0) then
      begin
        lConfig[i] := '  <add key="legacyBranchNum" value="' + InputPage.Values[0] + '" />';                  
        Break;
      end;
    end;
  
    lConfig.SaveToFile(ExpandConstant('{app}\appSettings.config'));    

  finally
    lConfig.Free();
  end;        
end;
