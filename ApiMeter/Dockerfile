FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY ./publish .
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "ApiMeter.dll"]

#FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
#RUN apt-get update && \
 #   apt-get install -y ca-certificates && \
 #  update-ca-certificates

#WORKDIR /src 
#set work dir inside container

#COPY *.csproj .
#Copy only project file first
#RUN dotnet restore 
#RUN dotnet restore --source  
#RUN --mount=type=cache,id=nuget,target=/root/.nuget/packages \
 #   dotnet restore
#Restore dependencies

#COPY . . 
#Copy the rest of the files

#RUN dotnet publish -c Release -o /app/publish
#Publish the application 

#Runtime stage
#FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
#WORKDIR /app 
#set work dir inside appln

#COPY --from=build /app/publish . 
#Copy the published files from build stage

#EXPOSE 8080 

#ENTRYPOINT ["dotnet", "ApiMeter.dll"] 
#Run the application


