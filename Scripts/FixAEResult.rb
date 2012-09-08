target_dir = File.dirname(__FILE__) + "/../Output/AE_Random_GreedyImproved"
Dir.glob(target_dir + "/*") do |dirname|
  puts dirname
  open(dirname + "/Flow.csv") do |f|
    open(dirname + "/FlowNZ.csv", "w") do |g|
      f.each_line do |line|
        unless line =~ /,0$/
          g.puts line
        end
      end
    end
  end
end
