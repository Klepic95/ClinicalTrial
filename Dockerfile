# Use the SDK image to build the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Add the project files
COPY ["ClinicalTrial.sln", "./"]
COPY ["ClinicalTrial.App/ClinicalTrial.App.csproj", "ClinicalTrial.App/"]
COPY ["ClinicalTrial.Application/ClinicalTrial.Application.csproj", "ClinicalTrial.Application/"]
COPY ["ClinicalTrial.Domain/ClinicalTrial.Domain.csproj", "ClinicalTrial.Domain/"]
COPY ["ClinicalTrial.Infrastructure/ClinicalTrial.Infrastructure.csproj", "ClinicalTrial.Infrastructure/"]
COPY ["ClinicalTrial.Presentation/ClinicalTrial.Presentation.csproj", "ClinicalTrial.Presentation/"]

RUN dotnet restore "ClinicalTrial.sln"

COPY . .

# Build the solution
RUN dotnet build "ClinicalTrial.sln" -c Release -o /app/build

# Publish the solution
RUN dotnet publish "ClinicalTrial.sln" -c Release -o /app/publish

# Use the runtime image for the final image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

COPY --from=build /app/publish ./

ENTRYPOINT ["dotnet", "ClinicalTrial.App.dll"]
