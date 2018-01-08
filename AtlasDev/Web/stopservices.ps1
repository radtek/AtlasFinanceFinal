$mainFilePath = "H:\Repositories"

function StartProcess ($exePath)
{
	$isProcessRunning = (get-process | ?{$_.path -eq $exePath}) 
	if ($isProcessRunning){
		kill $isProcessRunning.Id
	}
}

$payoutEngine = $mainFilePath + "\AtlasDev\Services\PayoutEngineServer\PayoutEngineServer\bin\Debug\Atlas.Payout.Engine.exe"
StartProcess -exePath $payoutEngine
$notificationServer = $mainFilePath + "\AtlasDev\Services\Atlas.Notification.Server\bin\Debug\Atlas.Notification.Server.exe"
StartProcess -exePath $notificationServer
$bureauServer = $mainFilePath + "\AtlasDev\Services\BureauServer\bin\Debug\Atlas.Bureau.Service.exe"
StartProcess -exePath $bureauServer
$avsEngineLight = $mainFilePath + "\AtlasDev\Services\AvsEngineLight\Server\bin\Debug\AvsEngineLight.exe"
StartProcess -exePath $avsEngineLight
$creditEngine = $mainFilePath + "\AtlasDev\Services\CreditEngine\bin\Debug\Atlas.Credit.Engine.exe"
StartProcess -exePath $creditEngine
$fraudEngine = $mainFilePath + "\AtlasDev\Services\FraudEngine\bin\Debug\Atlas.Fraud.Engine.exe"
StartProcess -exePath $fraudEngine
