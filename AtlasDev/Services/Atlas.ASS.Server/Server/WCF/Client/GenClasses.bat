﻿@REM Generate classes
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\SvcUtil.exe" https://www.nupaytsp.co.za/wsNaedo/wsNaedo.asmx                           /language:C# /ct:System.Collections.Generic.List`1  /serializer:XmlSerializer /o:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\wsNaedoSoap.cs"                  /config:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\wsNaedoSoap.config"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\SvcUtil.exe" https://www.nupay.co.za/WsNupayTransactions/NupayTransactionsService.asmx /language:C# /ct:System.Collections.Generic.List`1  /serializer:XmlSerializer /o:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\NuPayTransactionsServiceSoap.cs" /config:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\NuPayTransactionsServiceSoap.config"
"C:\Program Files (x86)\Microsoft SDKs\Windows\v10.0A\bin\NETFX 4.6.1 Tools\SvcUtil.exe" http://196.14.89.40/WS_TermRC_UAT/WS_TermRC.asmx?WSDL                     /language:C# /ct:System.Collections.Generic.List`1  /serializer:XmlSerializer /o:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\TermRCSoap.cs"                   /config:"D:\AtlasDev\Services\Atlas.ASS.Server\Server\WCF\Client\TermRCSoap.config"
