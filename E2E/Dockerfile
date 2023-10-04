FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["E2E/E2E.csproj", "E2E/"]
RUN dotnet restore "E2E/E2E.csproj"
COPY . .

WORKDIR "/src/E2E"
RUN dotnet build "E2E.csproj" -c Release -o /app/build

ARG API_URL
RUN echo ${API_URL}
ENV API_URL=${API_URL}

RUN dotnet test