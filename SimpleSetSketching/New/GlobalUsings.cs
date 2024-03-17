global using HashType = ulong;
global using ValueType = ulong;
global using HashingFunction = System.Func<ulong, ulong>;
global using HashingFunctionExpression = System.Linq.Expressions.Expression<System.Func<ulong, ulong>>;
global using HashingFunctions = System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression<System.Func<ulong, ulong>>>;
global using Set = System.Collections.Generic.HashSet<ulong>;

global using LittleSharp;
global using LittleSharp.Callables;
global using LittleSharp.Literals;
global using SimpleSetSketching.New.Hashing;
global using SimpleSetSketching.New.StreamProviders;
global using SimpleSetSketching.New.Utils;
global using SimpleSetSketching.New.Tooglers;


