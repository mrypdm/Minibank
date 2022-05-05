# Build

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS src

WORKDIR /src

COPY src .

RUN dotnet build MiniBank.Web -c Release -r linux-x64

RUN dotnet test Tests/MiniBank.Core.Tests --no-build

RUN dotnet publish MiniBank.Web -c Release -r linux-x64 -o /out --no-build

# Runtime

FROM mcr.microsoft.com/dotnet/aspnet:5.0 AS final

WORKDIR /app

COPY --from=src /out .

ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000

ENTRYPOINT ["dotnet", "MiniBank.Web.dll"]
