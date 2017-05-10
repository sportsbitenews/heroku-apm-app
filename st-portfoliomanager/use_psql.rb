require 'pg'

url = ENV['DATABASE_URL']
puts "Connecting to #{url}"

begin
  conn = PGconn.connect(url)
  res  = conn.exec("SELECT table_name FROM information_schema.tables WHERE table_schema='public' AND table_type='BASE TABLE';")
  puts "This database has the following tables:\n"
  res.each do |row|
    puts row['table_name']
  end

rescue => e
  abort "Failed to access database: #{e.message}"
end
