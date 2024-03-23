global using HashType = ulong;
global using ValueType = ulong;
global using HashingFunction = System.Func<ulong, ulong>;
global using HashingFunctionExpression = System.Linq.Expressions.Expression<System.Func<ulong, ulong>>;
global using HashingFunctions = System.Collections.Generic.IEnumerable<System.Linq.Expressions.Expression<System.Func<ulong, ulong>>>;
global using Set = System.Collections.Generic.HashSet<ulong>;

global using LittleSharp;
global using LittleSharp.Callables;
global using LittleSharp.Literals;
global using SimpleSetSketching.Hashing;
global using SimpleSetSketching.StreamProviders;
global using SimpleSetSketching.Utils;
global using SimpleSetSketching.Togglers;


