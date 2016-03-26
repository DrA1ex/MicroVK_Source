; Script generated by the Inno Setup Script Wizard.
; SEE THE DOCUMENTATION FOR DETAILS ON CREATING INNO SETUP SCRIPT FILES!

#define MyAppVersion "295"
#define MyAppVersionXP "264"

#define MyAppName "MicroVK" 
#define MyAppPublisher "MicroVK"
#define MyAppURL "https://vk.com/microvk"
#define MyAppExeName "MyProg.exe"
#define GitHubPath "D:\GitHub"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
; Do not use the same AppId value in installers for other applications.
; (To generate a new GUID, click Tools | Generate GUID inside the IDE.)
AppId={{FC957805-98B2-49EB-8620-7FC27B6A1BB6}
AppName={#MyAppName}
AppVersion="0.0.0.{#MyAppVersion}"
;AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
CreateAppDir=no
OutputDir=D:\MicroVKSetup
OutputBaseFilename=MicroVK_setup_{#MyAppVersion}
SetupIconFile=D:\Project\MicroVK\MicroVK\23.ico
Compression=lzma
SolidCompression=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"
Name: "russian"; MessagesFile: "compiler:Languages\Russian.isl"
Name: "ukrainian"; MessagesFile: "compiler:Languages\Ukrainian.isl" 

[Files]
Source: "{#GitHubPath}\MicroVK\MicroVK.application"; DestDir: "{tmp}\net46"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#GitHubPath}\MicroVK_XP\MicroVK.application"; DestDir: "{tmp}\net4"; Flags: ignoreversion recursesubdirs createallsubdirs

Source: "{#GitHubPath}\MicroVK\Application Files\MicroVK_0_0_0_{#MyAppVersion}\*"; DestDir: "{tmp}\net46\Application Files\MicroVK_0_0_0_{#MyAppVersion}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "{#GitHubPath}\MicroVK_XP\Application Files\MicroVK.XP_0_0_0_{#MyAppVersionXP}\*"; DestDir: "{tmp}\net4\Application Files\MicroVK.XP_0_0_0_{#MyAppVersionXP}"; Flags: ignoreversion recursesubdirs createallsubdirs
; NOTE: Don't use "Flags: ignoreversion" on any shared system files
;[run]
;Filename: "{tmp}\MicroVK.application"; Flags: shellexec

[code]
const
  deploymentProvider ='<deploymentProvider codebase="https://raw.githubusercontent.com/Kryeker/MicroVK/net46/MicroVK.application" />';
  deploymentProviderXP ='<deploymentProvider codebase="https://raw.github.com/Kryeker/MicroVK_XP/master/MicroVK.XP.application" />';
procedure CurStepChanged(CurStep: TSetupStep);
var UnicodeStr: string;
  Version: TWindowsVersion;
  ANSIStr: AnsiString;
  runFile:String;
  ResultCode: Integer;
  tempBool: Boolean;
  currentdeploymentProvider:String;
begin
  GetWindowsVersionEx(Version);
  if  CurStep=ssInstall  then
    begin

    end
  else if CurStep=ssPostInstall then
    begin  
      if Version.NTPlatform and (Version.Major >=6)  then
        begin
          runFile:=ExpandConstant('{tmp}\net46\MicroVK.application');
          currentdeploymentProvider:=deploymentProvider;
        end
      else
        begin
          runFile:=ExpandConstant('{tmp}\net4\MicroVK.XP.application');
          currentdeploymentProvider:=deploymentProvider;
        end;
      if LoadStringFromFile(runFile, ANSIStr) then
      begin    
        UnicodeStr := String(ANSIStr)       
        if StringChangeEx(UnicodeStr, currentdeploymentProvider, '', false)> 0 then;
          SaveStringToFile(runFile, UnicodeStr, False);
      end; 
      MsgBox(runFile, mbInformation, MB_OK);
      tempBool:= ShellExec('', runFile,'', '', SW_SHOW, ewNoWait, ResultCode)
    end;
end;
