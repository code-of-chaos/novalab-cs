# Generate the Development certificates for the various systems which need it
#      The default password is also used in the launch configurations

echo "Creating NovaLab.Server.pfx ..."
dotnet dev-certs https -ep %USERPROFILE%/.aspnet/https/NovaLab.Server.pfx -p pa55w0rd!

echo "Creating NovaLab.API.pfx ..."
dotnet dev-certs https -ep %USERPROFILE%/.aspnet/https/NovaLab.API.pfx -p pa55w0rd!