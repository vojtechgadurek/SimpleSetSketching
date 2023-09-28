namespace SimpleSetSketchingFSharp

module SimpleSetSketcher =
    let create (size : int) = Array.zeroCreate size


    let hashingFunction1 (element:uint64)= element

    let hashingFunction2 (element: uint64) =
        let a = (element ^^^ (element >>> 35)) * 0xbf58476d1de4e5b9UL >>> 33
        let b = (element ^^^ (element >>> 29)) * 0x94d049bb133111ebUL
        let ansver = (a ^^^ b ^^^ element) 
        ansver

    let hashingFunction3 (element: uint64) =
        let a = (element ^^^ (element >>> 28)) * 0x3C79AC482BA7B653UL >>> 33
        let b = element ^^^ ((element * 0x1C69B3F74AC4AE35UL) >>> 32)
        let ansver = (a ^^^ b ^^^ element) 
        ansver


    let toogle (sketchTable: array<uint64>) (mask:uint64) (hashingFunction1:(uint64 -> uint64)) (hashingFunction2:(uint64 -> uint64)) (hashingFunction3:(uint64 -> uint64)) (element:uint64) =
        let hash1 =(int) ((hashingFunction1 element) &&& mask)
        let hash2 =(int) ((hashingFunction2 element) &&& mask)
        let hash3 =(int) ((hashingFunction3 element) &&& mask)
        sketchTable[hash1] <- sketchTable[hash1] ^^^ element
        sketchTable[hash2] <- sketchTable[hash2] ^^^ element
        sketchTable[hash3] <- sketchTable[hash3] ^^^ element
        sketchTable
    let testToogle table element mask = toogle table mask hashingFunction1 hashingFunction2 hashingFunction3 element