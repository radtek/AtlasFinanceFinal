;
; Copyright(C) 2013-2014 - Atlas Finance (Pty) Ltd.
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
;
#define MyAppName "ASS Branch Data Sync Server"
#define MyAppVersion "Version 1.0"
#define MyAppPublisher "Atlas Finance (Pty) Ltd."
#define MyAppURL "http://www.atlasfinance.co.za"
#define MyAppExeName "ASSSyncClient.exe"
#define MyAppInstallPath "c:\Atlas\LMS\DataSync"
#include ReadReg(HKEY_LOCAL_MACHINE,'Software\Sherlock Software\InnoTools\Downloader','ScriptPath','') 

#define Version "1.0.0.0"
#define SourceAppPath "D:\AtlasDev\Services\Atlas.Data.ASSClient\wyUpdate\Release\V1.0.0.0"


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


[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"


[Dirs]
Name: {#MyAppInstallPath}; Permissions: everyone-full;


[Files]
Source: "{#SourceAppPath}\{#MyAppExeName}.config"; DestDir: {app}; AfterInstall: ConfigureConfig;

; Core
Source: "{#SourceAppPath}\ASSSyncClient.exe"; DestDir: {app};

; DLL's
Source: "{#SourceAppPath}\*.dll"; DestDir: {app}; 


[Run]
Filename: "{app}\{#MyAppExeName}"; Parameters: "install"; WorkingDir: "{app}"; Description: "Install service"; StatusMsg: "Installing the Atlas ASS Data Sync Service... please wait"
Filename: "{app}\{#MyAppExeName}"; Parameters: "start"; WorkingDir: "{app}"; Description: "Starting service"; StatusMsg: "Starting the Atlas ASS Data Sync Service... please wait"

[UninstallRun]
Filename: "{app}\{#MyAppExeName}"; Parameters: "stop"; WorkingDir: "{app}"
Filename: "{app}\{#MyAppExeName}"; Parameters: "uninstall"; WorkingDir: "{app}"


[Code]
var
  RequiresDOTNET: Boolean;                   // Does this machine require .NET?                                                  
  RequiresGDRUpdate: Boolean;                // Does this machine require the .NET GDR update?
  ProgressPage: TOutputProgressWizardPage;   // Custom dialog to show .NET installation in progress
  InputPage : TInputQueryWizardPage;
      
const
  APP_DIR = 'C:\Atlas\Client\Setup\';

  InputQueryPageID = 101;  // First created form is 100, second is 101...


  // .NET 4.0 Core installstion
  // --------------------------------------------------------------------------
  DOTNET_INSTALL_EXE = 'dotNetFx40_Full_x86_x64.EXE';
  DOTNET_FULL_PATH_EXE = APP_DIR + DOTNET_INSTALL_EXE;
  DOTNET_URL_2 = 'http://172.31.75.41/Software/Other/MS/' + DOTNET_INSTALL_EXE;
  DOTNET_URL_1 = 'http://172.31.75.38/Software/Other/MS/' + DOTNET_INSTALL_EXE;
  DOTNET_URL_3 = 'http://10.0.0.253/Software/Other/MS/' + DOTNET_INSTALL_EXE;
  DOTNET_URL_4 = 'http://download.microsoft.com/download/9/5/A/95A9616B-7A37-4AF6-BC36-D6EA96C8DAAE/' + DOTNET_INSTALL_EXE;

  // .NET GDR update (issues loading .net core dlls)
  // --------------------------------------------------------------------------
  WIN_DOTNET_1_UPDATE_INSTALL_EXE_32_BIT = 'NDP40-KB2468871-v2-x86.exe';
  WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT = APP_DIR + WIN_DOTNET_1_UPDATE_INSTALL_EXE_32_BIT;
  WIN_DOTNET_1_UPDATE_URL_1_32_BIT = 'http://172.31.75.41/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_32_BIT;
  WIN_DOTNET_1_UPDATE_URL_2_32_BIT = 'http://172.31.75.38/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_32_BIT;
  WIN_DOTNET_1_UPDATE_URL_3_32_BIT = 'http://10.0.0.253/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_32_BIT;

  WIN_DOTNET_1_UPDATE_INSTALL_EXE_64_BIT = 'NDP40-KB2468871-v2-x64.exe';
  WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT = APP_DIR + WIN_DOTNET_1_UPDATE_INSTALL_EXE_64_BIT;
  WIN_DOTNET_1_UPDATE_URL_1_64_BIT = 'http://172.31.75.41/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_64_BIT;
  WIN_DOTNET_1_UPDATE_URL_2_64_BIT = 'http://172.31.75.38/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_64_BIT;
  WIN_DOTNET_1_UPDATE_URL_3_64_BIT = 'http://10.0.0.253/Software/Other/MS/' + WIN_DOTNET_1_UPDATE_INSTALL_EXE_64_BIT;
  // --------------------------------------------------------------------------


  // Update 4.0.3 for Microsoft .NET Framework 4 – Runtime Update  
  // --------------------------------------------------------------------------
  WIN_DOTNET_2_UPDATE_INSTALL_EXE = 'NDP40-KB2600211-x86-x64.exe';
  WIN_DOTNET_2_UPDATE_FULL_PATH_EXE = APP_DIR + WIN_DOTNET_2_UPDATE_INSTALL_EXE;
  WIN_DOTNET_2_UPDATE_URL_1 = 'http://172.31.75.41/Software/Other/MS/' + WIN_DOTNET_2_UPDATE_INSTALL_EXE;
  WIN_DOTNET_2_UPDATE_URL_2 = 'http://172.31.75.38/Software/Other/MS/' + WIN_DOTNET_2_UPDATE_INSTALL_EXE;
  WIN_DOTNET_2_UPDATE_URL_3 = 'http://10.0.0.253/Software/Other/MS/' + WIN_DOTNET_2_UPDATE_INSTALL_EXE;  
  // --------------------------------------------------------------------------
  

function IsDotNetDetected(version: string; service: cardinal): boolean;
// Indicates whether the specified version and service pack of the .NET Framework is installed.
//
// version -- Specify one of these strings for the required .NET Framework version:
//    'v1.1.4322'     .NET Framework 1.1
//    'v2.0.50727'    .NET Framework 2.0
//    'v3.0'          .NET Framework 3.0
//    'v3.5'          .NET Framework 3.5
//    'v4\Client'     .NET Framework 4.0 Client Profile
//    'v4\Full'       .NET Framework 4.0 Full Installation
//
// service -- Specify any non-negative integer for the required service pack level:
//    0               No service packs required
//    1, 2, etc.      Service pack 1, 2, etc. required
var
  key: string;
  install, serviceCount: cardinal;
  success: boolean;
  
begin
  key := 'SOFTWARE\Microsoft\NET Framework Setup\NDP\' + version;
  // .NET 3.0 uses value InstallSuccess in subkey Setup
  if Pos('v3.0', version) = 1 then begin
    success := RegQueryDWordValue(HKLM, key + '\Setup', 'InstallSuccess', install);
  end else begin
    success := RegQueryDWordValue(HKLM, key, 'Install', install);
  end;
  
  // .NET 4.0 uses value Servicing instead of SP
  if Pos('v4', version) = 1 then begin
    success := success and RegQueryDWordValue(HKLM, key, 'Servicing', serviceCount);
  
  end else 
  begin
    success := success and RegQueryDWordValue(HKLM, key, 'SP', serviceCount);
  end;
  
  result := success and (install = 1) and (serviceCount >= service);
end;



function IsDOTNETGDRUpdateInstalled: Boolean;
begin
  Result := RegKeyExists(HKEY_LOCAL_MACHINE, 
    'SOFTWARE\Microsoft\Updates\Microsoft .NET Framework 4 Client Profile\KB2468871\');
end;


// Check .NET exists and force user to download the necessary MS setup files 
procedure InitializeWizard();
var
  lResultCode: Integer;

begin
  // Create .NET install progress page
  ProgressPage := CreateOutputProgressPage('Microsoft .NET 4.0- Installation & updates','');
  
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

  RequiresDOTNET := False;
    
  if not IsDotNetDetected('v4\Full', 0) then   
  begin
    RequiresDOTNET := True;
    
    ITD_Init();                                       // Initialize the downloader
    ITD_SetOption('UI_AllowContinue', '0');           // Do not allow to continue, if files cannot be downloaded
    ITD_SetOption('ITD_NoCache', '1');                // Do not use cached
    ITD_SetOption('UI_DetailedMode', '1');            // Show detailed GUI
    // Set the download caption (deprecated)
    ITD_SetOption('UI_Caption', 'Microsoft .NET 4.0 Framework download in progress');
    // Set download detail lavel
    ITD_SetOption('UI_Description', 'Downloading necessary additional software... please wait...');
    // Set failure message (deprecated)
    ITD_SetOption('UI_FailOrContinueMessage', 'Downloading of updates failed- please ensure you are connected to the Atlas network and press the ''Retry'' button to try the download again');
        
    ITD_DownloadAfter(wpReady); 
        
    // .NET 4.0 installer
    if (not FileExists(DOTNET_FULL_PATH_EXE)) then
    begin
      ITD_AddFile  (DOTNET_URL_1, DOTNET_FULL_PATH_EXE);
      ITD_AddMirror(DOTNET_URL_2, DOTNET_FULL_PATH_EXE);
      ITD_AddMirror(DOTNET_URL_3, DOTNET_FULL_PATH_EXE);
      ITD_AddMirror(DOTNET_URL_4, DOTNET_FULL_PATH_EXE);
    end;
    
    // .NET GDR
    RequiresGDRUpdate := True;
    if IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT) then
    begin
      ITD_AddFile  (WIN_DOTNET_1_UPDATE_URL_1_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_2_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_3_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);	
      
    end else if not IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT) then
    begin
      ITD_AddFile  (WIN_DOTNET_1_UPDATE_URL_1_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_2_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_3_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);		  
    end;
    
    // .NET 4.0.3
    if (not FileExists(WIN_DOTNET_2_UPDATE_FULL_PATH_EXE)) then
    begin    
      ITD_AddFile  (WIN_DOTNET_2_UPDATE_URL_1, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
      ITD_AddMirror(WIN_DOTNET_2_UPDATE_URL_2, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
      ITD_AddMirror(WIN_DOTNET_2_UPDATE_URL_3, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
    end;
        
    MsgBox('Atlas Branch DataSync requires the Microsoft .NET Framework V4.0, but has detected' + 
      'that this software has not been installed on this machine.' + #13#13 +
      'For the Atlas Branch DataSync software to function properly, this setup will download and '  +
      'install the Microsoft .NET Framework 4.0 (approx 49MB download) as well as critical .NET 4.0 updates (89-95MB).' + #13#13 +
      'NOTE: You *must* be connected to the internal Atlas network for these files to be downloaded successfully.', mbInformation, MB_OK);
  
  end else if not IsDOTNETGDRUpdateInstalled() then
  begin
    RequiresGDRUpdate := True;

    ITD_Init();     
    ITD_SetOption('UI_AllowContinue', '0');
    ITD_DownloadAfter(wpReady); 
    
    // .NET GDR
    if IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT) then
    begin
      ITD_AddFile  (WIN_DOTNET_1_UPDATE_URL_1_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_2_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_3_64_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT);	
      
    end else if not IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT) then
    begin
      ITD_AddFile  (WIN_DOTNET_1_UPDATE_URL_1_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_2_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);
      ITD_AddMirror(WIN_DOTNET_1_UPDATE_URL_3_32_BIT, WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT);		  
    end;
    
    // .NET 4.0.3
    if (not FileExists(WIN_DOTNET_2_UPDATE_FULL_PATH_EXE)) then
    begin    
      ITD_AddFile  (WIN_DOTNET_2_UPDATE_URL_1, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
      ITD_AddMirror(WIN_DOTNET_2_UPDATE_URL_2, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
      ITD_AddMirror(WIN_DOTNET_2_UPDATE_URL_3, WIN_DOTNET_2_UPDATE_FULL_PATH_EXE);
    end;       
      
    MsgBox('Atlas Branch DataSync has detected that your .NET installation requires critical Microsoft .NET updates.' + #13#10 +
      'For the Atlas Branch DataSync software to function properly, this setup will download and '  +
      'install necessary Microsoft .NET Framework 4.0 updates (approx. 89-95MB download).' + #13#13 +
      'NOTE: You *must* be connected to the internal Atlas network, or the Internet ' +
      'for this file to be downloaded successfully.', mbInformation, MB_OK);
  end; 
end;


procedure CurStepChanged(CurStep: TSetupStep);
var
  lResultCode: Integer;

begin
  if (CurStep = ssInstall) then // Run .NET installs 
  begin
    if (RequiresDOTNET) then
    begin            
      ProgressPage.Show();
      ProgressPage.SetText('.NET framework checks... Please wait...', '');
      
      try        
        ProgressPage.SetProgress(0, 100);

        // Check files exist
        // ------------------------------------------------------------------------------------------------------------        
        if not FileExists(DOTNET_FULL_PATH_EXE) then
        begin
          MsgBox('The Microsoft .NET Framework 4.0 installer failed to download!', mbInformation, MB_OK);
          Abort();
        end;         
                
        ProgressPage.SetText('Installing the Microsoft .NET framework... Please wait...', '');                    
        if not Exec(DOTNET_FULL_PATH_EXE, '/passive /norestart', APP_DIR, SW_SHOW, ewWaitUntilTerminated, lResultCode) then
        begin          
          MsgBox('Failed to start the Microsoft .NET Framework 4.0 install. Error: "' + SysErrorMessage(lResultCode) + '"', 
            mbInformation, MB_OK);
          Abort();
        end;
        
        if (lResultCode <> 0) then
        begin
          MsgBox('The Microsoft .NET Framework 4.0 installer failed/was aborted. Exit code: [' + IntToStr(lResultCode) + ']', 
            mbInformation, MB_OK);
          Abort();
        end;

        RequiresDOTNET := False;

      finally              
        ProgressPage.Hide();
      end;
    end;

    if (RequiresGDRUpdate) then
    begin      
      ProgressPage.Show();
      
      try
        ProgressPage.SetText('Installing Microsoft .NET 4.0 GDR update... Please wait...', '');
        ProgressPage.SetProgress(20, 100);
        
        if IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT) then
        begin
          MsgBox('The Microsoft .NET Framework 4.0 GDR (64-bit) installer failed to download!', mbInformation, MB_OK);
          Abort();
        end;

        if not IsWin64() and not FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT) then
        begin
          MsgBox('The Microsoft .NET Framework 4.0 GDR (32-bit) installer failed to download!', mbInformation, MB_OK);
          Abort();
        end;
        
        // .NET GDR
        if IsWin64() and FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT) then
        begin
          if not Exec(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_64_BIT, '/passive /norestart /showfinalerror', APP_DIR, SW_SHOW, ewWaitUntilTerminated, lResultCode) then
          begin
            MsgBox('Failed to start the .NET GDR update. Error: "' + SysErrorMessage(lResultCode) + '"', 
              mbInformation, MB_OK);
            Abort();
          end;
        
          if (lResultCode <> 0) then
          begin
            MsgBox('The .NET GDR update setup failed/was aborted. Exit code: [' + IntToStr(lResultCode) + ']', 
              mbInformation, MB_OK);   
            Abort();
          end;
        end; 

        if not IsWin64() and FileExists(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT) then
        begin
          if not Exec(WIN_DOTNET_1_UPDATE_FULL_PATH_EXE_32_BIT, '/passive /norestart /showfinalerror', APP_DIR, SW_SHOW, ewWaitUntilTerminated, lResultCode) then
          begin
            MsgBox('Failed to start the .NET GDR update. Error: "' + SysErrorMessage(lResultCode) + '"', 
              mbInformation, MB_OK); 
            Abort();
          end;
        
          if (lResultCode <> 0) then
          begin
            MsgBox('The .NET GDR update setup failed/was aborted. Exit code: [' + IntToStr(lResultCode) + ']', 
              mbInformation, MB_OK);       
            Abort();
          end;
        end;
		
		    if FileExists(WIN_DOTNET_2_UPDATE_FULL_PATH_EXE) then
        begin
          ProgressPage.SetText('Installing Microsoft .NET 4.0.3 update... Please wait...', '');
          ProgressPage.SetProgress(60, 100);

          if not Exec(WIN_DOTNET_2_UPDATE_FULL_PATH_EXE, '/passive /norestart /showfinalerror', APP_DIR, SW_SHOW, ewWaitUntilTerminated, lResultCode) then
          begin
            MsgBox('Failed to start the .NET 4.0.3 update. Error: "' + SysErrorMessage(lResultCode) + '"', 
              mbInformation, MB_OK);            
            Abort();
          end;
          
          if (lResultCode <> 0) then
          begin
            MsgBox('The .NET 4.0.3 update setup failed/was aborted. Exit code: [' + IntToStr(lResultCode) + ']', 
              mbInformation, MB_OK);       
            Abort();
          end;
        end

        RequiresGDRUpdate := False;
        ProgressPage.SetProgress(100, 100);
                    
      finally        
        ProgressPage.Hide();        
      end;
    end; 
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
  lFound: Boolean;
 
begin  
  lConfig := TStringList.Create();
  try
    lConfig.LoadFromFile(ExpandConstant('{app}\ASSSyncClient.exe.config'));    
    
    for i:= 0 to lConfig.Count - 1 do
    begin
      if (Pos('add key="legacybranchnum"', Lowercase(lConfig[i])) > 0) then
      begin
        lConfig[i] := '    <add key="legacyBranchNum" value="' + InputPage.Values[0] + '" />';                  
        Break;
      end;
    end;
  
    lConfig.SaveToFile(ExpandConstant('{app}\ASSSyncClient.exe.config'));    

  finally
    lConfig.Free();
  end;        
end;