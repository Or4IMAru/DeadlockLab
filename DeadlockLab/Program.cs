namespace DeadlockLab;

internal class Program
{
    static async Task Main()
    {
        Console.WriteLine("DEADLOCK И БЕЗОПАСНОЕ ВЗАИМОДЕЙСТВИЕ");
        Console.WriteLine();

        Console.WriteLine("1. Воспроизведение deadlock");
        Console.WriteLine("2. Безопасные банковские переводы");
        Console.WriteLine("3. Эксперимент с Mutex и lock-порядком");
        Console.WriteLine();

        Console.Write("Выберите эксперимент: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                DeadlockDemo.Run();
                break;

            case "2":
                await TransferDemo.RunSafeAsync();
                break;

            case "3":
                Console.WriteLine(
                    "Запустите два перевода в противоположных направлениях."
                );

                await TransferDemo.RunSafeAsync();
                break;

            default:
                Console.WriteLine("Неизвестная команда.");
                break;
        }
    }
}
