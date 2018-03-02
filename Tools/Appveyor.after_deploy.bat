﻿@echo on

@SET /A exitcode=0
@SET /A TESTFAILURE_ERROR=1
@SET /A PUSHARTIFACT_ERROR=2
@SET /A READREPORT_ERROR=4

nunit3-console.exe Tests\CSF.Screenplay.WebApis.Tests\bin\Debug\CSF.Screenplay.WebApis.Tests.dll
@IF %ERRORLEVEL% NEQ 0 SET /A exitcode^|=%TESTFAILURE_ERROR%

appveyor PushArtifact SpecFlow.report.txt
@IF %ERRORLEVEL% NEQ 0 SET /A exitcode^|=%PUSHARTIFACT_ERROR%
appveyor PushArtifact JsonApis.report.txt
@IF %ERRORLEVEL% NEQ 0 SET /A exitcode^|=%PUSHARTIFACT_ERROR%

@echo ******************
@echo Screenplay reports
@echo ******************

@type SpecFlow.report.txt
@IF %ERRORLEVEL% NEQ 0 SET /A exitcode^|=%READREPORT_ERROR%
@type JsonApis.report.txt
@IF %ERRORLEVEL% NEQ 0 SET /A exitcode^|=%READREPORT_ERROR%

@echo off

@EXIT /B %exitcode%
