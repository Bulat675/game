using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp2
{
    class Program
    {
        private static int arrows;

        // Структура для хранения информации о комнате
        struct Room
        {
            public string Event; // Тип события в комнате
            public int MonsterHealth; // Здоровье монстра (если есть)
            public int TrapDamage; // Урон от ловушки (если есть)
            public string Riddle; // Загадка для сундука (если есть)
            public string RiddleAnswer; // Ответ на загадку
            public int Reward; // Награда за решение загадки (0 - ничего, 1 - зелье, 2 - золото, 3 - стрелы)
            public int GoldReward; //Награда золотом
        }
        static void Main(string[] args)
        {
            // Игровые характеристики
            int health = 100;
            int potions = 3;
            int gold = 0;
            int arrows = 5;
            string[] inventory = new string[5];
            int inventoryCount = 0;
            // Карта подземелья (10 комнат)
            Room[] dungeonMap = GenerateDungeon();

            // Игровой цикл
            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine($"\nВы входите в комнату {i + 1}.");
                ProcessRoom(dungeonMap[i], ref health, ref potions, ref gold, ref arrows, inventory, ref inventoryCount);
                if (health <= 0)
                {
                    Console.WriteLine("Вы погибли!");
                    return;
                }
                if (i == 9 && dungeonMap[i].Event == "Босс")
                {
                    Console.WriteLine("Вы победили босса! Вы выиграли!");
                    return;
                }
            }
        }

        // Генерация случайной карты подземелья
        static Room[] GenerateDungeon()
        {
            Room[] dungeonMap = new Room[10];
            string[] events = { "Монстр", "Ловушка", "Сундук", "Торговец", "Пустая комната" };
            Random random = new Random();

            for (int i = 0; i < 9; i++)
            {
                dungeonMap[i].Event = events[random.Next(events.Length)];
                if (dungeonMap[i].Event == "Монстр")
                {
                    dungeonMap[i].MonsterHealth = random.Next(20, 51);
                }
                else if (dungeonMap[i].Event == "Ловушка")
                {
                    dungeonMap[i].TrapDamage = random.Next(10, 21);
                }
                else if (dungeonMap[i].Event == "Сундук")
                {
                    dungeonMap[i].Riddle = GenerateRiddle(out dungeonMap[i].RiddleAnswer, out dungeonMap[i].Reward, out dungeonMap[i].GoldReward);
                }

            }
            dungeonMap[9].Event = "Босс";
            dungeonMap[9].MonsterHealth = 100; // Здоровье босса

            return dungeonMap;
        }

        // Обработка события в комнате
        static void ProcessRoom(Room room, ref int health, ref int potions, ref int gold, ref int arrows, string[] inventory, ref int inventoryCount)
        {
            switch (room.Event)
            {
                case "Монстр":
                    FightMonster(room.MonsterHealth, ref health, ref arrows);
                    break;
                case "Ловушка":
                    Trap(room.TrapDamage, ref health);
                    break;
                case "Сундук":
                    OpenChest(room.Riddle, room.RiddleAnswer, ref potions, ref gold, ref arrows, inventory, ref inventoryCount);
                    break;
                case "Торговец":
                    VisitMerchant(ref potions, ref gold);
                    break;
                case "Пустая комната":
                    Console.WriteLine("Пустая комната.");
                    break;
                case "Босс":
                    FightBoss(ref health);
                    break;
            }
        }

        // Бой с монстром
        static void FightMonster(int monsterHealth, ref int health, ref int arrows)
        {
            Random random = new Random();
            while (monsterHealth > 0 && health > 0)
            {
                Console.WriteLine($"\nЗдоровье монстра: {monsterHealth}, Ваше здоровье: {health}");
                Console.WriteLine("Выберите оружие:");
                Console.WriteLine("1. Меч");
                Console.WriteLine("2. Лук");
                int choice = int.Parse(Console.ReadLine());

                int damage = 0;
                if (choice == 1)
                {
                    damage = random.Next(10, 21);
                    monsterHealth -= damage;
                    Console.WriteLine($"Вы нанесли {damage} урона!");
                }
                else if (choice == 2 && arrows > 0)
                {
                    damage = random.Next(5, 16);
                    monsterHealth -= damage;
                    arrows--;
                    Console.WriteLine($"Вы нанесли {damage} урона! Осталось стрел: {arrows}");
                }
                else
                {
                    Console.WriteLine("У вас нет стрел!");
                }

                if (monsterHealth <= 0) break;

                int monsterDamage = random.Next(5, 16);
                health -= monsterDamage;
                Console.WriteLine($"Монстр нанёс вам {monsterDamage} урона!");
            }
            if (monsterHealth <= 0) Console.WriteLine("Вы победили монстра!");
        }


        //Попасть в ловушку
        static void Trap(int trapDamage, ref int health)
        {
            health -= trapDamage;
            Console.WriteLine($"Вы попали в ловушку и потеряли {trapDamage} здоровья!");
        }


        //Открытие сундука
        static void OpenChest(string riddle, string answer, ref int potions, ref int gold, ref int arrows, string[] inventory, ref int inventoryCount)
        {
            Console.WriteLine($"\nПеред вами сундук! Чтобы его открыть, решите загадку:");
            Console.WriteLine(riddle);
            string playerAnswer = Console.ReadLine();

            if (playerAnswer.ToLower() == answer.ToLower())
            {
                Random random = new Random();
                int rewardType = random.Next(3); // 0 - ничего, 1 - зелье, 2 - золото, 3 - стрелы
                int rewardAmount = random.Next(1, 4); // количество награды
                if (rewardType == 1 && inventoryCount < 5)
                {
                    potions += rewardAmount;
                    inventory[inventoryCount] = "Зелье";
                    inventoryCount++;
                    Console.WriteLine($"Вы нашли {rewardAmount} зелье(йя)!");
                }
                else if (rewardType == 2)
                {
                    gold += rewardAmount * 10; // Золота больше
                    Console.WriteLine($"Вы нашли {rewardAmount * 10} золота!");
                }
                else if (rewardType == 3 && inventoryCount < 5)
                {
                    arrows += rewardAmount;
                    inventory[inventoryCount] = "Стрелы";
                    inventoryCount++;
                    Console.WriteLine($"Вы нашли {rewardAmount} стрел!");
                }
                else
                {
                    Console.WriteLine("В сундуке ничего нет.");
                }
            }
            else
            {
                Console.WriteLine("Неверный ответ!");
            }
        }

        // Посещение торговца
        static void VisitMerchant(ref int potions, ref int gold)
        {
            Console.WriteLine("\nВы встретили торговца.");
            Console.WriteLine("У него можно купить зелье за 30 золота.");
            Console.WriteLine("У вас {0} золота.", gold);
            if (gold >= 30)
            {
                Console.WriteLine("Купить зелье? (да/нет)");
                string answer = Console.ReadLine();
                if (answer.ToLower() == "да")
                {
                    gold -= 30;
                    potions++;
                    Console.WriteLine("Вы купили зелье!");
                }
            }
            else
            {
                Console.WriteLine("У вас недостаточно золота.");
            }
        }

        // Бой с боссом
        static void FightBoss(ref int health)
        {
            // ... (реализация боя с боссом аналогична бою с монстром)
            Console.WriteLine("\nВы встретили могущественного босса!");
            Random random = new Random();
            int bossHealth = 100;
            while (bossHealth > 0 && health > 0)
            {
                Console.WriteLine($"\nЗдоровье босса: {bossHealth}, Ваше здоровье: {health}");
                Console.WriteLine("Выберите оружие:");
                Console.WriteLine("1. Меч");
                Console.WriteLine("2. Лук");
                int choice = int.Parse(Console.ReadLine());

                int damage = 0;
                if (choice == 1)
                {
                    damage = random.Next(10, 21);
                    bossHealth -= damage;
                    Console.WriteLine($"Вы нанесли {damage} урона!");
                }
                else if (choice == 2)
                {
                    if (arrows > 0)
                    {
                        damage = random.Next(5, 16);
                        bossHealth -= damage;
                        arrows--;
                        Console.WriteLine($"Вы нанесли {damage} урона! Осталось стрел: {arrows}");
                    }
                    else
                    {
                        Console.WriteLine("У вас нет стрел!");
                    }
                }

                if (bossHealth <= 0) break;

                int bossDamage = random.Next(10, 21); // Босс бьёт сильнее
                health -= bossDamage;
                Console.WriteLine($"Босс нанёс вам {bossDamage} урона!");
            }
            if (bossHealth <= 0) Console.WriteLine("Вы победили босса!");
        }

        // Генерация загадки
        static string GenerateRiddle(out string answer, out int reward, out int goldReward)
        {
            Random random = new Random();
            int riddleNumber = random.Next(3);
            string riddle = "";
            answer = "";
            reward = 0;
            goldReward = 0;

            switch (riddleNumber)
            {
                case 0:
                    riddle = "Что имеет голову, хвост, но не имеет тела?";
                    answer = "Монета";
                    goldReward = 20;
                    break;
                case 1:
                    riddle = "Что можно сломать, но нельзя держать?";
                    answer = "Обещание";
                    reward = 1;
                    break;
                case 2:
                    riddle = "Я всегда перед тобой, но ты не можешь меня увидеть. Что я?";
                    answer = "Будущее";
                    reward = 3;
                    break;

            }
            return riddle;
        }
    }
    }

