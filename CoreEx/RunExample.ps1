cls

#cleanup
docker system prune -f
docker rm -f core
docker rmi core-img

#build
docker build --tag core-img . 

#run
docker run -d --name core -p 8086:80 -e application_name="xtra_core_test" -e DD_API_KEY="" -e DD_LOG_ENDPOINT="https://http-intake.logs.datadoghq.com" -e BuildNumber="1.0.0.1" core-img 

 #enter running container
docker ps -a
