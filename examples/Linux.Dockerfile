FROM mcr.microsoft.com/dotnet/sdk:6.0-focal AS build

COPY examples/AspNetCoreExample/AspNetCoreExample.csproj examples/AspNetCoreExample/
COPY src/WKPDFGen/WKPDFGen.csproj src/WKPDFGen/

RUN dotnet restore examples/AspNetCoreExample/AspNetCoreExample.csproj --verbosity n

COPY . .

RUN dotnet publish examples/AspNetCoreExample/AspNetCoreExample.csproj --no-restore --configuration Release --output out --verbosity n

FROM mcr.microsoft.com/dotnet/aspnet:6.0-focal

RUN apt-get update && apt-get install -y \
    zlib1g \ 
    fontconfig \
    libfreetype6 \
    libx11-6 \
    libxext6 \
    libxrender1 \
    libgdiplus \
    libxcb1 \
    xfonts-75dpi \
    xfonts-base

COPY --from=build /out /app

ENTRYPOINT ["dotnet", "/app/AspNetCoreExample.dll"]