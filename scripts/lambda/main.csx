Func<string, string, string> unisciNomi = (n1, n2) => {
    return nome1 + " & "  + nome2; 
};
string nome1 = "pippo"; string nome2 = "pluto";
string result = unisciNomi(nome1, nome2);
Console.WriteLine(result);


Func<int, int, int, int> prendiMaggiore = (pr, se, te) => {
    return Math.Max(pr, Math.Max(se, te));;
};
int prima = 5; int seconda = 7; int terza = 1;
int result2 = prendiMaggiore(prima, seconda, terza);
Console.WriteLine(result2);


Action<DateTime, DateTime> stampaData = (d1, d2) => {
    if (d1 <= d2)
    { Console.WriteLine(d2); }
    else
    { Console.WriteLine(d1); }
};
DateTime data1 = new DateTime(1998, 06, 12); DateTime data2 = new DateTime(2001, 06, 17);
stampaData(data1, data2);

Action<DateTime> printDate = date => Console.WriteLine(date);
DateTime date = DateTime.Today;
printDate(date);