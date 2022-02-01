#base image
FROM mcr.microsoft.com/dotnet/framework/aspnet:4.8-windowsservercore-ltsc2019 AS base
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
# Download and install the tracer
# We recommend always using the latest release and regularly updating: https://github.com/DataDog/dd-trace-dotnet/releases/latest
ARG DD_TRACER_VERSION=2.1.1
ENV DD_TRACER_VERSION=$DD_TRACER_VERSION
ENV ASPNETCORE_URLS=http://*.80
 
ENV COR_ENABLE_PROFILING="1"
ENV COR_PROFILER="{846F5F1C-F9AE-4B07-969E-05C26BC060D8}"
 
ENV CORECLR_ENABLE_PROFILING="1"
ENV CORECLR_PROFILER=$COR_PROFILER
 
RUN Write-Host "Downloading Datadog .NET Tracer v$env:DD_TRACER_VERSION" ;\
    (New-Object System.Net.WebClient).DownloadFile('https://github.com/DataDog/dd-trace-dotnet/releases/download/v' + $env:DD_TRACER_VERSION + '/datadog-dotnet-apm-' + $env:DD_TRACER_VERSION + '-x64.msi', 'datadog-apm.msi') ;\
    Write-Host 'Installing Datadog .NET Tracer' ;\
    Start-Process -Wait msiexec -ArgumentList '/i datadog-apm.msi /quiet /qn /norestart /log datadog-apm-msi-installer.log' ;\
    Write-Host 'Datadog .NET Tracer installed, removing installer file' ;\
    Remove-Item 'datadog-apm.msi'
 
WORKDIR /inetpub/wwwroot
EXPOSE 80

#build image
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019 AS build
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]
WORKDIR /src
COPY . .
RUN nuget restore FrameworkEx.sln
RUN MSBuild FrameworkEx.sln /p:RunCodeAnalysis=False /p:DeployOnBuild=true /p:PublishProfile=filepublish /p:publishUrl='c:\app' /p:Configuration=Release

#runtime image
FROM base AS runtime
COPY --from=build /app .