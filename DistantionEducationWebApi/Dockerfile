FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
COPY requirements1.txt .
RUN apt-get update -y
RUN apt-get install python3-pip python3 -y
RUN pip install --user -r requirements1.txt --break-system-packages
RUN python3 -m nltk.downloader stopwords
COPY ./TextAnalisys /TextAnalisys
WORKDIR /app
EXPOSE 8080
EXPOSE 8081
EXPOSE 8082

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["DistantionEducationWebApi.csproj", ""]
RUN dotnet restore "./DistantionEducationWebApi.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./DistantionEducationWebApi.csproj" -c Debug -o /app/build

FROM build AS publish
RUN dotnet publish "./DistantionEducationWebApi.csproj" -c Debug -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "DistantionEducationWebApi.dll"]