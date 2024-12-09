var diskFiles = new List<(int ptr, int id, int size)>();
var diskSpaces = new List<(int ptr, int size)>();

void Init() {
    var diskMap = File.ReadAllText("input.txt")
                      .Select((o, i) => (isFile: i % 2 == 0, size: o - '0'))
                      .ToList();
    var ptr = 0;

    foreach (var i in Enumerable.Range(0, diskMap.Count)) {
        (var isFile, var size) = diskMap[i];

        if (isFile) {
            diskFiles.Add((ptr, diskFiles.Count, size));
        } else {
            diskSpaces.Add((ptr, size));
        }

        ptr += size;
    }
}

long P1() {
    var spacesStack = new Stack<(int ptr, int size)>(diskSpaces.AsEnumerable().Reverse());
    var filesStack = new Stack<(int ptr, int id, int size)>(diskFiles);
    var movedFiles = new List<(int ptr, int id, int size)>();

    while (spacesStack.TryPop(out var space)) {
        (var spacePtr, var spaceSize) = space;

        if (spaceSize == 0)
            continue;

        if (spacePtr > filesStack.Peek().ptr)
            break;

        (var filePtr, var fileId, var fileSize) = filesStack.Pop();

        if (spaceSize == fileSize) {
            movedFiles.Add((spacePtr, fileId, fileSize));
        } else if (spaceSize > fileSize) {
            movedFiles.Add((spacePtr, fileId, fileSize));
            spacesStack.Push((spacePtr + fileSize, spaceSize - fileSize));
        } else {
            movedFiles.Add((spacePtr, fileId, spaceSize));
            filesStack.Push((filePtr, fileId, fileSize - spaceSize));
        }
    }

    return filesStack.Concat(movedFiles)
                     .Sum(o => Enumerable.Range(o.ptr, o.size)
                                         .Select(ptr => ptr * (long)o.id)
                                         .Sum());
}

long P2() {
    var spaces = diskSpaces.ToList();
    var movedFiles = new List<(int ptr, int id, int size)>();

    foreach (var i in Enumerable.Range(1, diskFiles.Count)) {
        (var filePtr, var fileId, var fileSize) = diskFiles[^i];

        if (spaces.Count == 0 || spaces[0].ptr > filePtr)
            break;

        foreach (var j in Enumerable.Range(0, spaces.Count)) {
            (var spacePtr, var spaceSize) = spaces[j];

            if (spacePtr > filePtr)
                break;

            if (spaceSize >= fileSize) {
                movedFiles.Add((spacePtr, fileId, fileSize));

                if (spaceSize == fileSize)
                    spaces.RemoveAt(j);
                else
                    spaces[j] = (spacePtr + fileSize, spaceSize - fileSize);

                break;
            }
        }
    }

    var filesDic = movedFiles.ToDictionary(o => o.id, o => o);

    foreach (var left in diskFiles.Where(o => !filesDic.ContainsKey(o.id)))
        filesDic[left.id] = left;

    return filesDic.Values.Sum(o => Enumerable.Range(o.ptr, o.size)
                                              .Select(ptr => ptr * (long)o.id)
                                              .Sum());
}

Init();
Console.WriteLine(P1());
Console.WriteLine(P2());