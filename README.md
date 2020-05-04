# Truck Loading Software Benchmarks
Benchmarking the Java and C# implementations of the truck loading optimization algorithm

# How to use
This solution provides benchmarks for the Java and C# implementations of the optimization algorithm. In order to run the benchmark on your PC, you need to open it in Visual Studio (developed in VS 2019) and set the build environment to **Release**.

# C# Port notes
This is only a proof-of-concept port of the Java algorithm. The code is more or less 1:1 translation of the original codebase, except for the CSV reading mechanism, which has been selected for it's ease of use and rapid development, rather than performance. As such, there are no C# specific performane optimizations, which will have an effect in the final release version.

# Benchmark results
Summary:

OS=Windows 10.0.18362.778 (1903/May2019Update/19H1)

Intel Core i7-9700K CPU 3.60GHz (Coffee Lake), 1 CPU, 8 logical and 8 physical cores

.NET Core SDK=3.1.101

[Host]: .NET Core 3.1.1 (CoreCLR 4.700.19.60701, CoreFX 4.700.19.60801), X64 RyuJIT

Job=.NET Core 3.1  
Runtime=.NET Core 3.1

| Method |    Mean |    Error |   StdDev | Ratio |
|------- |--------:|---------:|---------:|------:|
|    Jar | 1.137 s | 0.0224 s | 0.0306 s |  1.00 |
|        |         |          |          |       |
| Ported | 1.919 s | 0.0375 s | 0.0695 s |  1.00 |
**Lower is better**

However, there's a skew in the Java statistics here. It appears that the Java VM struggles to clear out it's memmory after rapid sequential executions of the jar file, which crashes the benchmark execution. I had to add a 200ms delay between the jar executions to give the JVM time to flush out it's memory. This means that we need to **shave 200ms** from the **Mean Jar execution time** and the actual benchmark results are the following:

| Method |    Mean |    Error |   StdDev | Ratio |
|------- |--------:|---------:|---------:|------:|
|    Jar | 0.932 s | 0.0224 s | 0.0306 s |  1.00 |
|        |         |          |          |       |
| Ported | 1.919 s | 0.0375 s | 0.0695 s |  1.00 |
**Lower is better**

**Legend**

**Jar**: The original Java code, being executed through a command prompt script.

**Ported**: The ported C# code, executed through the standard C# console runtime.

**Mean**: Arithmetic mean of all measurement

**Error**: Half of 99.9% confidence interval

**StdDev**: Standard deviation of all measurements

**Ratio**: Mean of the ration distribution ([Current]/[Baseline])

# Benchmark Result Notes
- **No** language specific performance optimizations were implemented in the C# port of the codebase.
- The CSV reading mechanism in the C# port is very inneficient (it adds about ~**600ms** of overhead to the execution). The Java CSV reader only adds ~90ms of overhead to the execution. In reality this means that once the CSV reader is optimized in the C# port, we will shave about 500ms of the C# execution time.
- The JVM struggles to clear out memory after each run, which could mean crashes and skewed results when executed rapidly. In reality, this won't happen very often, however it's still a concern.

# Pros and cons of each approach
### C# Pros and cons
#### Pros
- The intended final implementation of the C# code is to do away with all the input/output overhead we currently have with the Java implemention, since it's going to be running on the actual back-end server and we won't have to transform JSON into CSV on-disk, read the CSV, run the calculations, save back to CSV on-disk, parse into JSON in the back-end and then return the result. The indended approach is to remove the CSV overhead from the C# implementation altogether.
- Integrating the solution in the C# back-end would allow us to fairly easily make the code asynchronous. Essentially, we would be able to handle more requests at the same time since we won't be using blocking code to finish executing.
- Scaling across multiple machines (if ever needed) would be much easier to handle with the in-place C# implementation.
- Much more familiar technology for the team.
- An integrated approach to exception handling, monitoring and maintaining will be much easier if something breaks or needs to change down the road.
#### Cons
- A fair amount of effort in order to optimize performance.
- Additional development time needed in order to "translate" the Java codebase.
- A mechanism to test the output of both solutions would need to be implemented, in order to ensure the correct execution of the C# port.

### Java Pros and cons
#### Pros
- Initial implementation faster than the C# one.
- Developed by the algorithm author.
### Cons
- A lot of input/output and extra-process communication overhead.
- Blocking calls due to the nature of the integration with the C# back-end. Potentially less requests served per second, even though algorithm runtime is faster.
- Much harder to maintain and troubleshoot down the road, due to the black-box nature of the application. Collecting logs and crash reports is going to be a convoluted process, which would lean to a longer bugfixing and maintaining time.
- Unfamiliar technology for the team.

# Final assesment
In my opinion, the C# pros outweigh the cons significantly. We can improve performance impactfully when we remove the CSV parsing mechanism, even though further performance optimizations would be much less obvious and difficulty to implement. I would advise that we proceed with integrating the final version of the algorithm into the C# back-end codebase, rather than call it as a separate black-box. In order to ensure the correct operation of the ported version, however, we would need to be able to test the output of each algorithm implementation against one another. Since the current implementation heavily depends on data randomization, I suggest implementing a configuration flag that disables the randomization of the data, which we could then use to create reproducible and predictible outputs from both algorithm and test the C# version against the Java version for correctness.