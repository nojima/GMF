target_dir1 = File.dirname(__FILE__) + "/../Output/AE_Random_GreedyImproved"
target_dir2 = File.dirname(__FILE__) + "/../Output/AE_Random_Greedy"

values = {}
times = {}
for dir in [target_dir1, target_dir2]
  Dir.glob(dir + "/*") do |dirname|
    k = File.basename(dirname).to_i
    open(dirname + "/Value.txt") do |f|
      value = f.read.to_f
      if values[k]
	values[k].push(value)
      else
	values[k] = [value]
      end
    end
    open(dirname + "/Summary.txt") do |f|
      f.each_line do |line|
        if line =~ /^GMF Time \[ms\]: (\d+)$/
          if times[k]
            times[k].push($1)
          else
            times[k] = [$1]
          end
        end
      end
    end
  end
end

for id, vs in values.sort
  next if vs.size < 2
  puts "#{id},#{vs.join(',')},#{vs[1] / vs[0]}"
end
puts
for id, vs in times.sort
  next if vs.size < 2
  puts "#{id},#{vs.join(',')}"
end
