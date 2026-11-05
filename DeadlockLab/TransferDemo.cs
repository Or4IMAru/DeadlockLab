using System;
using System.Threading.Tasks;

namespace DeadlockLab;

public class TransferDemo
{
    public static async Task RunSafeAsync()
    {
        Console.WriteLine("=== SAFE TRANSFER (10 000 операций) ===");
        BankAccount accountA = new(1000);
        BankAccount accountB = new(2000);
        BankTransferService service = new();

        const int operations = 10000;
        const decimal amount = 1;

        Task[] tasks = new Task[2];

        tasks[0] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe(accountA, accountB, amount);
            }
        });

        tasks[1] = Task.Run(() =>
        {
            for (int i = 0; i < operations; i++)
            {
                service.TransferSafe(accountB, accountA, amount);
            }
        });

        await Task.WhenAll(tasks);

        decimal totalBalance = accountA.Balance + accountB.Balance;
        Console.WriteLine($"Баланс A: {accountA.Balance}");
        Console.WriteLine($"Баланс B: {accountB.Balance}");
        Console.WriteLine($"Общий баланс: {totalBalance} (Ожидалось: 3000)");
        Console.WriteLine(totalBalance == 3000 ? "✅ УСПЕХ: Баланс сошёлся!\n" : "❌ ОШИБКА: Баланс не сошёлся!\n");

        Console.WriteLine("=== ПРОВЕРКА НЕДОСТАТКА СРЕДСТВ ===");
        BankAccount accPoor = new(100);
        BankAccount accRich = new(1000);
        decimal initialTotal = accPoor.Balance + accRich.Balance;

        try
        {
            service.TransferSafe(accPoor, accRich, 500);
            Console.WriteLine("❌ ОШИБКА: Исключение не было выброшено!");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"✅ Ожидаемое исключение: {ex.Message}");
        }

        decimal afterFailTotal = accPoor.Balance + accRich.Balance;
        Console.WriteLine($"Баланс Poor: {accPoor.Balance}, Баланс Rich: {accRich.Balance}");
        Console.WriteLine($"Общий баланс после сбоя: {afterFailTotal} (Ожидалось: {initialTotal})");
        Console.WriteLine(initialTotal == afterFailTotal
            ? "✅ УСПЕХ: Транзакция атомарна, частичного изменения состояния не произошло!\n"
            : "❌ ОШИБКА: Произошло частичное обновление баланса!\n");

        Console.WriteLine("=== SAFE TRANSFER (3 счёта, циклические переводы) ===");
        BankAccount accX = new(1000);
        BankAccount accY = new(1000);
        BankAccount accZ = new(1000);

        Task[] tasks3 = new Task[3];
        int ops3 = 5000;

        tasks3[0] = Task.Run(() =>
        {
            for (int i = 0; i < ops3; i++) service.TransferSafe(accX, accY, 1);
        });
        tasks3[1] = Task.Run(() =>
        {
            for (int i = 0; i < ops3; i++) service.TransferSafe(accY, accZ, 1);
        });
        tasks3[2] = Task.Run(() =>
        {
            for (int i = 0; i < ops3; i++) service.TransferSafe(accZ, accX, 1);
        });

        await Task.WhenAll(tasks3);

        decimal total3 = accX.Balance + accY.Balance + accZ.Balance;
        Console.WriteLine($"Баланс X: {accX.Balance}, Y: {accY.Balance}, Z: {accZ.Balance}");
        Console.WriteLine($"Общий баланс: {total3} (Ожидалось: 3000)");
        Console.WriteLine(total3 == 3000
            ? "✅ УСПЕХ: Deadlock не возник благодаря сортировке по Id, баланс сошёлся!\n"
            : "❌ ОШИБКА!\n");

        Console.WriteLine("=== ПРОВЕРКА ПЕРЕВОДА САМОМУ СЕБЕ ===");
        BankAccount accSelf = new(500);
        decimal beforeSelf = accSelf.Balance;

        service.TransferSafe(accSelf, accSelf, 100);

        decimal afterSelf = accSelf.Balance;
        Console.WriteLine($"Баланс до: {beforeSelf}, после: {afterSelf}");
        Console.WriteLine(beforeSelf == afterSelf
            ? "✅ УСПЕХ: Перевод самому себе безопасно проигнорирован!"
            : "❌ ОШИБКА: Баланс изменился!");
    }
}