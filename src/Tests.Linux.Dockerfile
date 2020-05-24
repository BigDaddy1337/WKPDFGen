FROM mcr.microsoft.com/dotnet/core/sdk:3.1  AS build-stage

COPY WKPDFGen/WKPDFGen.csproj WKPDFGen/
COPY WKPDFGen.Tests/WKPDFGen.Tests.csproj WKPDFGen.Tests/

RUN dotnet restore WKPDFGen.Tests/WKPDFGen.Tests.csproj --verbosity n

COPY . .

RUN dotnet test --no-restore