//Per fare il debug di questi esempi, devi prima installare il global tool dotnet-script con questo comando:
//dotnet tool install -g dotnet-script
//Trovi altre istruzioni nel file /scripts/readme.md
class Apple {
    public string Color { get; set; }
    public int Weight { get; set; } //In grammi
}

List<Apple> apples = new List<Apple> {
    new Apple { Color = "Red", Weight = 180 },
    new Apple { Color = "Green", Weight = 145 },
    new Apple { Color = "Red", Weight = 190 },
    new Apple { Color = "Green", Weight = 185 },
    new Apple { Color = "Red", Weight = 200 },
};

//ESEMPIO #1: Ottengo i pesi delle mele rosse
IEnumerable<int> weightsOfRedApples = apples
                            .Where(Apple => Apple.Color == "Red")
                            .Select(Apple => Apple.Weight);

//ESEMPIO #2: Calcolo la media dei pesi ottenuti
double average = weightsOfRedApples.Average();
Console.WriteLine(average);

//ESERCIZIO #1: Qual è il peso minimo delle 4 mele?
//int minimumWeight = apples...;
int PesoMinimo = apples.Min(apples => apples.Weight);
Console.WriteLine(PesoMinimo);


//ESERCIZIO #2: Di che colore è la mela che pesa 190 grammi?
//string color = apples...;
IEnumerable<string> colore = apples
                .Where(Apple => Apple.Weight == 190)
                .Select(apple => apple.Color);
Console.WriteLine(colore.First());

string colore1 = apples
                .Where(Apple => Apple.Weight == 190)
                .Select(apple => apple.Color).First();
Console.WriteLine(colore1);


//ESERCIZIO #3: Quante sono le mele rosse?
//int redAppleCount = apples...;
int ContaRosse = apples.Where(Apple => Apple
                .Color == "Red")
                .Count();
Console.WriteLine(ContaRosse);



//ESERCIZIO #4: Qual è il peso totale delle mele verdi?
//int totalWeight = apples...;
int PesoTotale = apples.Where(Apple => Apple
                .Color == "Green")
                .Sum(apples => (apples.Weight));
Console.WriteLine("Peso totale " + PesoTotale);