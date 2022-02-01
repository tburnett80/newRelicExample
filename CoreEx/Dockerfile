FROM mcr.microsoft.com/dotnet/aspnet:5.0 as base
WORKDIR /datadog
 
# Install the latest version of the tracer
ARG DD_TRACER_VERSION=2.1.1
ENV DD_TRACER_VERSION=$DD_TRACER_VERSION
RUN curl -Lo datadog-dotnet-apm.zip https://github.com/DataDog/dd-trace-dotnet/releases/download/v%DD_TRACER_VERSION%/windows-tracer-home.zip \
  && tar.exe -xf datadog-dotnet-apm.zip \
  && del datadog-dotnet-apm.zip
 
# Enable the tracer
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={846F5F1C-F9AE-4B07-969E-05C26BC060D8}
ENV CORECLR_PROFILER_PATH=/datadog/Datadog.Trace.ClrProfiler.Native.dll
ENV DD_DOTNET_TRACER_HOME=/datadog
ENV DD_INTEGRATIONS=/datadog/integrations.json
 
WORKDIR /app
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY . .
RUN dotnet restore CoreEx.sln
RUN dotnet publish CoreEx.sln -c Release -o /app


FROM base as runtime
COPY --from=build /app .
ENTRYPOINT ["dotnet", "CoreEx.dll"]
