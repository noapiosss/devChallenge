FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Contracts/Contracts.csproj", "Contracts/"]
COPY ["E2E/E2E.csproj", "E2E/"]
RUN dotnet restore "E2E/E2E.csproj"
COPY . .

WORKDIR "/src/E2E"
RUN dotnet build "E2E.csproj" --no-restore

CMD ["dotnet", "test", "--no-build"]
