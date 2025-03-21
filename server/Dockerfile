FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["API/API.csproj", "API/"]
COPY ["Infrastructure/Infrastructure.csproj", "Infrastructure/"]
COPY ["Core/Core.csproj", "Core/"]
RUN dotnet restore "API/API.csproj"

COPY . .

EXPOSE 5203

CMD ["dotnet", "run", "--project", "API/API.csproj", "--urls", "http://0.0.0.0:5203"]