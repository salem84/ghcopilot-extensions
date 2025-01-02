main() {
    echo 'Install SWA CLI...'
    npm install --global @azure/static-web-apps-cli

    echo 'Install .NET 8 for DAB...'
    wget https://packages.microsoft.com/config/debian/12/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
    sudo dpkg -i packages-microsoft-prod.deb
    rm packages-microsoft-prod.deb
    sudo apt-get update
    sudo apt-get install -y dotnet-sdk-8.0

    echo 'Install DAB CLI...'
    dotnet tool install --global Microsoft.DataApiBuilder
}

# Start
main