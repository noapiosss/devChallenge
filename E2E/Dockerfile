# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
# WORKDIR /E2E
# EXPOSE 80
# EXPOSE 443

# FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
# WORKDIR /src
# COPY ["E2E/E2E.csproj", "E2E/"]
# RUN dotnet restore "E2E/E2E.csproj"
# COPY . .
# WORKDIR "/src/E2E"
# RUN dotnet build "E2E.csproj" -c Release -o /E2E/build

# FROM base AS final
# WORKDIR /E2E
# COPY --from=build /E2E/build .
# ENTRYPOINT ["dotnet", "test", "E2E.dll"]


FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["E2E/E2E.csproj", "E2E/"]
RUN dotnet restore "E2E/E2E.csproj"
COPY . .

WORKDIR "/src/E2E"
RUN dotnet build "E2E.csproj" -c Release -o /app/build
RUN dotnet test

# WORKDIR "/src/E2E"
# RUN dotnet publish "E2E.csproj" -c Release -o /app/publish

# FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
# WORKDIR /app
# COPY --from=build /app/publish .
# EXPOSE 80
# ENTRYPOINT ["dotnet", "test", "E2E.dll"]