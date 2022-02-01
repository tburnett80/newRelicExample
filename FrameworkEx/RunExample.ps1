cls

#cleanup
docker system prune -f
docker rm -f framework
docker rmi framework-img

#build
docker build --tag framework-img . 

#run
docker run -d --name framework -p 8085:80 `
 -e DD_API_KEY="temp" `
 -e DD_LOG_ENDPOINT="https://http-intake.logs.datadoghq.com" `
 -e BuildNumber="1.0.0.1" `
 framework-img 

 #enter running container
#docker exec -it framework powershell
