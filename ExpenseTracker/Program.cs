// =====================================================================
//  Program.cs  —  the interactive console UI for MyBudget (Assignment 1).
//  Target framework: .NET 10 (LTS), language C# 14.
//
//  >>> BUILD THE MENU-DRIVEN UI HERE (Modules 1-3). <<<
//
//  Once you have implemented BudgetRules.cs (so the unit tests pass), wire it
//  up to a console interface that meets the assignment brief:
//
//    * Print a banner (try a raw string literal).
//    * Loop a menu until the user exits, using a switch on the choice:
//        1) Add an expense   2) View summary   3) Set monthly budget   4) Exit
//    * Read and VALIDATE input, re-prompting on bad data (decimal.TryParse,
//      BudgetRules.NormalizeCategory, a date parse, non-empty text).
//    * Keep running totals in simple variables (no collections / no classes).
//    * Use BudgetRules.ValidateAmount / ClassifyAmount / BudgetStatus /
//      FormatCurrency for all logic and formatting.
//    * Handle bad input with try / catch / finally and InvalidExpenseException.
//
//  See section 6 of the assignment brief for a sample run to aim for.
// =====================================================================
using ExpenseTracker;

Console.WriteLine($"""
============================================================
MyBudget Expense Tracker
Today: {DateOnly.FromDateTime(DateTime.Today)}
============================================================
""");

// Overall expense tracking
decimal totalExpenses = 0m;
int expenseCount = 0;
decimal highestExpense = 0m;

// Monthly budget
decimal monthlyBudget = 0m;
bool budgetIsSet = false;

// Category totals
decimal foodTotal = 0m;
decimal transportTotal = 0m;
decimal utilitiesTotal = 0m;
decimal entertainmentTotal = 0m;
decimal otherTotal = 0m;

// Menu control
bool isRunning = true;

while (isRunning)
{
    Console.WriteLine();
    Console.WriteLine("1) Add an expense");
    Console.WriteLine("2) View summary");
    Console.WriteLine("3) Set monthly budget");
    Console.WriteLine("4) Exit");
    Console.Write("Enter your choice: ");

    string? choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            {
                try
                {
                    //Description cannot be empty Method
                    string description = ReadRequiredText("Expense description: ");

                    //Amount must be realistic and positive
                    decimal amount = ReadPositiveAmount("Amount: $");

                    //Category must be valid
                    string category = ReadCategory();

                    //Date format and validation
                    DateOnly expenseDate = ReadExpenseDate();


                    //Optional note
                    Console.Write("Optional note (press Enter to skip): ");
                    string? noteInput = Console.ReadLine();

                    string? note = string.IsNullOrWhiteSpace(noteInput)
                        ? null
                        : noteInput.Trim();

                    // Update the overall totals.
                    totalExpenses += amount;
                    expenseCount++;

                    if (amount > highestExpense)
                    {
                        highestExpense = amount;
                    }

                    // Update the total for the selected category.
                    switch (category)
                    {
                        case "Food":
                            foodTotal += amount;
                            break;

                        case "Transport":
                            transportTotal += amount;
                            break;

                        case "Utilities":
                            utilitiesTotal += amount;
                            break;

                        case "Entertainment":
                            entertainmentTotal += amount;
                            break;

                        case "Other":
                            otherTotal += amount;
                            break;
                    }

                    Console.WriteLine();
                    Console.WriteLine("Expense added successfully!");
                    Console.WriteLine($"Description: {description}");
                    Console.WriteLine($"Amount: {BudgetRules.FormatCurrency(amount)}");
                    Console.WriteLine($"Category: {category}");
                    Console.WriteLine($"Date: {expenseDate:yyyy-MM-dd}");
                    //Classify and display amount size
                    Console.WriteLine($"Size: {BudgetRules.ClassifyAmount(amount)}");
                    Console.WriteLine($"Note: {note ?? "None"}");
                }
                catch (InvalidExpenseException ex)
                {
                    Console.WriteLine($"Expense error: {ex.Message}");
                }
                finally
                {
                    Console.WriteLine("Expense entry process finished.");
                }

                break;
            }



        case "2":
            Console.WriteLine("View summary selected.");
            break;


        case "3":
            Console.WriteLine("Set monthly budget selected.");
            break;

        case "4":
            isRunning = false;
            Console.WriteLine("Thank you for using MyBudget!");
            break;

        default:
            Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 4.");
            break;
    }
}


static string ReadRequiredText(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!string.IsNullOrWhiteSpace(input))
        {
            return input.Trim();
        }

        Console.WriteLine("This field cannot be empty. Please try again.");
    }
}

static decimal ReadPositiveAmount(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal amount))
        {
            Console.WriteLine("Please enter a valid number, such as 12.50.");
            continue;
        }

        try
        {
            return BudgetRules.ValidateAmount(amount);
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}

static string ReadCategory()
{
    while (true)
    {
        Console.Write("Category (Food, Transport, Utilities, Entertainment, Other): ");
        string? input = Console.ReadLine();

        string? category = BudgetRules.NormalizeCategory(input);

        if (category is not null)
        {
            return category;
        }

        Console.WriteLine("Invalid category. You can also enter F, T, U, E, or O.");
    }
}

static DateOnly ReadExpenseDate()
{
    while (true)
    {
        Console.Write("Date (yyyy-mm-dd, or press Enter for today): ");
        string? input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            return DateOnly.FromDateTime(DateTime.Today);
        }

        if (!DateOnly.TryParseExact(
            input.Trim(),
            "yyyy-MM-dd",
            System.Globalization.CultureInfo.InvariantCulture,
            System.Globalization.DateTimeStyles.None,
            out DateOnly expenseDate))
        {
            Console.WriteLine("Invalid date format. Please use yyyy-mm-dd.");
            continue;
        }

        try
        {
            if (expenseDate > DateOnly.FromDateTime(DateTime.Today))
            {
                throw new InvalidExpenseException(
                    "Future dates are not allowed.");
            }

            return expenseDate;
        }
        catch (InvalidExpenseException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
}


