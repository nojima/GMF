target_dir = File.dirname(__FILE__) + "/../Output/AccuracyExperimentCompressed"

values = [
0.001953125,
0.015625,
0.001953125,
0.03125,
0.040807599852071,
0.8495498285060974,
0.170565119760479,
0.0512551700367647,
0.03501958819241985,
0.0078125,
0.0078125,
0.0078125,
0.0,
0.02574666341145835,
0.00390625,
0.03108867994100295,
0.0,
0.0,
0.01562499999999999,
0.0078125,
0.0078125,
0.031088679941002936,
0.00048828125,
0.0019531249999999987,
0.000244140625,
0.015625,
0.0779400887573965,
0.015625,
0.03125,
0.015625,
0.0078125,
0.0078125,
0.0625,
0.0009765625,
0.5,
0.0078125,
0.038879751461988334,
0.00390625,
0.00390625,
0.125,
0.125,
0.00390050551470588,
0.00048828125,
0.046691715542521994,
6.103515625e-05,
0.015625,
0.015625,
0.015625,
0.00780101102941175,
0.02715992647058825,
]

result = []
Dir.glob(target_dir + "/*") do |dirname|
  source = sink = value = nil
  next unless File.exist?(dirname + "/Value.txt")
  open(dirname + "/Sunnary.txt") do |f|
    f.each_line do |line|
      if line =~ /^Source: (\d+)$/
        source = $1.to_i
      elsif line =~ /^Sink: (\d+)$/
        sink = $1.to_i
      end
    end
  end
  open(dirname + "/Value.txt") do |f|
    value = f.read.to_f
  end
  result.push [File.basename(dirname).to_i, source, sink, value]
end
result.sort.each do |xs|
  puts "{" + xs.join(',') + "}, # ratio = #{xs[3] / values[xs[0]]}"
end
