ARG DOTNET_RUNTIME=mcr.microsoft.com/dotnet/aspnet:8.0
ARG DOTNET_SDK=mcr.microsoft.com/dotnet/sdk:8.0

# Stage 1: Build the Blazor application
FROM ${DOTNET_SDK} AS build
ARG BUILD_CONFIGURATION=Release

COPY ./src/NovaLab/NovaLab.csproj ./src/NovaLab/NovaLab.csproj 
COPY . .

WORKDIR ./src/NovaLab
RUN dotnet restore
RUN dotnet build -c $BUILD_CONFIGURATION --no-restore -o /app/build

# Stage 2: Publish the application
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "../NovaLab/NovaLab.csproj" -c $BUILD_CONFIGURATION -o /app/publish/NovaLab 

# Stage 3: Create the runtime image
FROM ${DOTNET_SDK} AS base
WORKDIR /app
COPY --from=publish /app/publish .
COPY entrypoint.sh .
COPY tools.sh .

RUN chmod +x ./entrypoint.sh
RUN chmod +x ./tools.sh

EXPOSE 443

ENTRYPOINT ["./entrypoint.sh"]