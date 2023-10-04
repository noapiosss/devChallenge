FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["Api/Api.csproj", "Api/"]
RUN dotnet restore "Api/Api.csproj"
COPY . .

WORKDIR "/src/Api"
RUN dotnet build "Api.csproj" -c Release -o /app/build
RUN dotnet tool install --global dotnet-ef --version 7.0.0
RUN dotnet publish "Api.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll", "--environment=Production"]