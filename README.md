# LineServer

#### How to run the system?

1.  Download & install [.Net 8.0 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
2. Run `./build.sh` (Optional)
3. Run `./generate_file.sh <file-path> <file-size>` to generate a file (Optional)
    - this runs a small console app that generates a file for the system;
    - file size is always in GB and won't take decimals;
    - example `./generate_file.sh 100gb_file.txt 100`.
4. Run `./run.sh <file-name>`
    - this will start up the web api, using the file passed as parameter;
    - example `./run.sh 100gb_file.txt`.
5. Run `./performance_tests.sh <api-url> <lines-total>` (Optional)
    - this will run performance tests on the web api, using NBomber;
    - lines total represents the biggest line index that can be requested, to avoid bad requests errors since we want to test the performance of the web api and not it's functionality;
    - example `./performance_tests.sh http://localhost:5292/ 99999999`.


#### How does your system work? (if not addressed in comments in source)

- The system preloads each line index into a collection
    - The class responsible for this is the `LineIndexer`;
    - It will use a `StreamReader` to iterate the file and add the stream position on the line index collection for every `\n` character it finds;
    - Only runs once per system startup.
- Two approaches where considered:
    - `FileReaderWithMemoryMappedFile` (**main approach**):
        - Singleton object that initializes a `MemoryMappedFile` on the constructor.
        - For every request, it will:
            - check if the index is valid, not beyond the end of the file;
            - get the stream position from the line index collection;
            - calculates the length of the line by using the next line position, or the total file length;
            - creates a view stream with the position and length required for the single line;
            - use a `StreamReader` to read the line from the file and return it.
        - Advantages: 
            - Each thread can run independently, can scale well with multiple concurrent users;
            - Utilises OS memory paging, providing lower response times.
                - File is mapped into virtual memory and only parts of the file are loaded into physical memory.
        - Cons:
            - Higher memory usage than the other approach due to memory mapping.
    - `FileReaderWithFileStream` (**first approach but outperformed by the main**):
        - Singleton object that initializes a `FileStream` and a `Semaphore` on the constructor.
        - For every request, it will:
            - check if the index is valid, not beyond the end of the file;
            - get the stream position from the line index collection;
            - update the `FileStream` position to the position of the requested line index;
            - use a `StreamReader` to read the line from the file and return it;
            - utilises the `Semaphore`to control the access to the `FileStream` since it is shared between threads.
        - Advantages: 
            - Reutilization of `FileStream` to avoid reopening the file on every request;
            - Low memory usage since `StreamReader` only loads chunks of the file and when requested.
        - Cons:
            - Increased response time due to `Semaphore` lock;
            - Not the best scalable solution for concurrent requests due to `Semaphore` lock.

#### How will your system perform with a 1 GB file? a 10 GB file? a 100 GB file? && How will your system perform with 100 users? 10000 users? 1000000 users?

I ran some performance tests with NBomber. 1 per approach mentioned and 1 per file size (1GB, 10GB and 40GB, since my machine ran out of space to use a 100GB file) The NBomber setup simulated 2000 users during 30 seconds. I wasn't able to run more than this due to machine limitations.

The full results can be found on [reports folder](reports).

I'll resume the main take away points looking at the 40gb file run:
- the `MemoryMappedFile` approach used more memory than the `FileStream` approach, picking at 360MB while the second on picked at 170MB;
- the `MemoryMappedFile` approach used more CPU than the `FileStream` approach, picking at 12% while the second on picked at 1.4%;
- the `MemoryMappedFile` approach was able to use more threads than the `FileStream` approach, picking at 22 while the second on picked at 18;
- the `MemoryMappedFile` approach was faster than the `FileStream` approach, getting a P99 of ~70-88 seconds while the second got a P99 of ~94-132 seconds;
- the `MemoryMappedFile` approach received more RPS than the `FileStream` approach, getting a constant ~52000 RPS while the second picked at ~25000 RPS.
    - this is related to the NBomber setup, constant 2000 users does not relate to constant 2000 requests, since each user will constantly try to execute more requests.

In conclusion, the `MemoryMappedFile` approach received more RPS because it was able to maximize the number of threads and execute requests faster. This can correlate to the increased memory and CPU usage. To understand if this the actual cause of it, I would need to make more performance tests where the RPS would be constant for both approaches. Nevertheless, I believe this is enough data to confirm that the `MemoryMappedFile` approach performs better for more users and for bigger files. Even for smaller files, we can reach the same conclusions since the reports showcase a small memory increase with this approach but with more RPS as well.

#### What documentation, websites, papers, etc did you consult in doing this assignment?

- [.Net Memory Mapped File](https://learn.microsoft.com/en-us/dotnet/standard/io/memory-mapped-files)
- [.Net performance tests](https://learn.microsoft.com/en-us/aspnet/core/test/load-tests?view=aspnetcore-9.0)
- [ChatGPT](https://chatgpt.com/)

#### What third-party libraries or other tools does the system use? How did you choose each library or framework you used?

- [NBomber](https://nbomber.com/)
    - One of Microsoft's recommended third-party tools for performance/load tests;
    - Ease of use;
    - Capable of simulating multiple scenarios.

#### How long did you spend on this exercise? If you had unlimited more time to spend on this, how would you spend it and how would you prioritize each item? && If you were to critique your code, what would you have to say about it?

Spent around 12 hours. If I had unlimited more time, I would like to try a few other approaches/possible improvements, in priority order:
1. Change index preload logic to save the index collection into the disk;
    - the system can check if the index collection already exists and, if the file did not change, we can use the already existent index collection, accelerating the startup;
    - Reason: can significantly improve our system startup for bigger file.
2. Explore how the system would perform if I split the file into multiple smaller files
    - For the Memory Mapped File approach, I don't think it would improve the performance since it works best with bigger files;
    - However, for the File Stream approach, it could improve alot since that would also allow multiple concurrent threads, one per file;
    - Reason: can improve the performance but it is not a given, we would need to test it out.
3. Cache most requested lines or next lines in order
    - Reason: can improve performance but is dependant on user behaviour
4. Enrich the strategy pattern for the algorithms with a factory that will select the approach based on file size
    - Reason: can improve performance by choosing the best approach for the size of the file
5. Change the index preload logic to not block the system startup, but instead return an error on the endpoints that require it until the preload finishes.
    - Reason: won't make a difference in the current form of the api since we only have one endpoint and it depends on the index preload, but could improve api availability in the future (with more endpoints that don't require index preload)
