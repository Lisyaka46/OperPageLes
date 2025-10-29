namespace OperPageLes.CORE
{
    /// <summary>
    /// Класс сортировки элементов
    /// </summary>
    internal sealed class Sorting
    {
        /// <summary>
        /// Отсортировать в массиве строки
        /// </summary>
        /// <param name="Names">Массив строк</param>
        public static void SortNames(ref string[] Names)
        {
            char[][] CharMassName = [.. Names.Select((i) => i.ToArray())];
            int Index, IndexActivate = CharMassName.Length - 1;
            for (int i = 0; i < CharMassName.Length; i++)
            {
                Index = CharMassName.Length - 1;
                for (int j = Index - 1; j > -1; j--)
                {
                    if (CharMassName[IndexActivate].Length != CharMassName[j].Length)
                    {
                        if (CharMassName[IndexActivate].Length < CharMassName[j].Length) Index--;
                        continue;
                    }
                    for (int k = 0; k < CharMassName[IndexActivate].Length; k++)
                    {
                        if (CharMassName[IndexActivate][k] != CharMassName[j][k])
                        {
                            if (CharMassName[IndexActivate][k] < CharMassName[j][k]) Index--;
                            break;
                        }
                    }
                }
                if (IndexActivate != Index)
                {
                    (CharMassName[IndexActivate], CharMassName[Index]) = (CharMassName[Index], CharMassName[IndexActivate]);
                    continue;
                }
            }
            Names = [.. CharMassName.Select((i) => new string(i))];
        }
    }
}
