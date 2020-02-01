docker stop single-sign-on 
docker rm single-sign-on  
docker run \
	--name=single-sign-on \
	-d --restart unless-stopped \
	-p 5001:80 \
	-e "ConnectionStrings__DefaultConnection"="Host=localhost;Database=SingleSignOn;Username=postgres;Password=password1;" \
	-e "DatabaseType"="Postgres" \
	laredoza/single-sign-on:latest \
	--restart unless-stopped