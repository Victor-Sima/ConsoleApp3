using System;
using System.Collections.Generic;
using System.IO;

namespace ManagementStudenti
{
    class Student
    {
        private string nume;
        private string prenume;
        private string email;
        private DateTime dataNasterii;
        private DateTime dataInscriere;
        private bool absolvit;

        public Student(string nume, string prenume, string email, DateTime dataNasterii, DateTime dataInscriere) // Modificare în constructor
        {
            this.nume = nume;
            this.prenume = prenume;
            this.email = email;
            this.dataNasterii = dataNasterii;
            this.dataInscriere = dataInscriere;
            this.absolvit = false;
        }
        public string Nume { get { return nume; } }
        public string Prenume { get { return prenume; } }
        public string Email { get { return email; } }
        public DateTime DataNasterii { get { return dataNasterii; } }
        public DateTime DataInscriere { get { return dataInscriere; } }
        public bool Absolvit { get { return absolvit; } }

        public void SetAbsolvit(bool absolvit)
        {
            this.absolvit = absolvit;
        }
        public string ToFileLine(string numeFacultate)
        {
            return nume + "," + prenume + "," + email + "," + dataNasterii.ToString("yyyy-MM-dd") + "," + dataInscriere.ToString("yyyy-MM-dd") + "," + (absolvit ? true : false) + "," + numeFacultate;
        }
    }
    class Facultate
    {
        public string Nume { get; }
        public string Abreviere { get; }
        public string Domeniu { get; }
        private List<Student> studenti;

        public Facultate(string nume, string abreviere, string domeniu)
        {
            Nume = nume;
            Abreviere = abreviere;
            Domeniu = domeniu;
            studenti = new List<Student>();
        }
        public void AdaugaStudent(Student student)
        {
            studenti.Add(student);
        }

        public List<Student> GetStudenti()
        {
            return studenti;
        }
        public void AfiseazaStudenti()
        {
            foreach (Student student in studenti)
            {
                Console.WriteLine("Numele: " + student.Nume + "\nPrenumele: " + student.Prenume + "\nEmail: " + student.Email);
            }
        }
        public void SalveazaStudenti(string numeFisier, string numeFacultate)
        {
            File.WriteAllText(numeFisier, string.Empty);

            using (StreamWriter writer = new StreamWriter(numeFisier, true))
            {
                foreach (Student student in studenti)
                {
                    writer.WriteLine(student.ToFileLine(numeFacultate));
                }
            }
        }
        public void AfiseazaFacultateInfo()
        {
            Console.WriteLine("Nume facultate: " + Nume);
            Console.WriteLine("Abreviere facultate: " + Abreviere);
            Console.WriteLine("Domeniu facultate: " + Domeniu);
        }
    }
    class SistemManagementStudenti
    {
        internal Dictionary<string, Facultate> facultati;
        private List<Student> studentiNeatribuiti;
        private StreamWriter logWriter;
        public SistemManagementStudenti()
        {
            this.facultati = new Dictionary<string, Facultate>();
            this.studentiNeatribuiti = new List<Student>();
            try
            {
                IncarcaFacultati("C:\\Users\\victo\\Desktop\\ConsoleApp3\\facultati.txt");
                IncarcaStudenti("C:\\Users\\victo\\Desktop\\ConsoleApp3\\studenti.txt");
                this.logWriter = new StreamWriter("C:\\Users\\victo\\Desktop\\ConsoleApp3\\operatii.txt", true);
                InregistreazaOperatie("\nExecutiile:\nIntrare în program");
            }
            catch (IOException e)
            {
                Console.WriteLine("Eroare la deschiderea fisierului de log: " + e.Message);
            }
        }
        public void AfiseazaStudentiNeabsolviti(string numeFacultate)
        {
            if (facultati.ContainsKey(numeFacultate))
            {
                Facultate facultate = facultati[numeFacultate];
                Console.WriteLine("Studentii inscrisi:");
                foreach (Student student in facultate.GetStudenti())
                {
                    if (!student.Absolvit)
                    {
                        Console.WriteLine(student.Nume + " " + student.Prenume + " - " + student.Email);
                    }
                }
                InregistreazaOperatie("Afisare studenti neabsolviti pentru facultatea " + numeFacultate);
            }
            else
            {
                Console.WriteLine("Facultatea nu exista.");
            }
        }
        public void AfiseazaStudentiFacultate(string numeFacultate)
        {
            if (facultati.ContainsKey(numeFacultate))
            {
                Facultate facultate = facultati[numeFacultate];
                facultate.AfiseazaStudenti();

                InregistreazaOperatie("Afisare studenti absolventi pentru facultatea " + numeFacultate);
            }
            else
            {
                Console.WriteLine("Facultatea nu exista.");
            }
        }
        private void IncarcaFacultati(string numeFisier)
        {
            foreach (string linie in File.ReadLines(numeFisier))
            {
                string[] parts = linie.Split(',');
                if (parts.Length >= 3)
                {
                    string numeFacultate = parts[0];
                    string abreviereFacultate = parts[1];
                    string domeniuFacultate = parts[2];
                    facultati.Add(numeFacultate, new Facultate(numeFacultate, abreviereFacultate, domeniuFacultate));
                }
            }
        }

        private void IncarcaStudenti(string numeFisier)
        {
            foreach (string linie in File.ReadLines(numeFisier))
            {
                string[] parts = linie.Split(',');
                if (parts.Length >= 6)
                {
                    string nume = parts[0];
                    string prenume = parts[1];
                    string email = parts[2];
                    DateTime dataNasterii = DateTime.Parse(parts[3]);
                    DateTime dataInscriere = DateTime.Parse(parts[4]);
                    bool absolvit = bool.Parse(parts[5]);
                    string numeFacultate = parts[6];
                    Student student = new Student(nume, prenume, email, dataNasterii, dataInscriere);
                    student.SetAbsolvit(absolvit);
                    if (facultati.ContainsKey(numeFacultate))
                    {
                        facultati[numeFacultate].AdaugaStudent(student);
                    }
                    else
                    {
                        studentiNeatribuiti.Add(student);
                    }
                }
                else
                {
                    Console.WriteLine("Linie incorecta in fisierul studenti.txt: " + linie);
                }
            }
        }
        public void CreeazaFacultate(string nume, string abreviere, string domeniu)
        {
            facultati.Add(nume, new Facultate(nume, abreviere, domeniu));
            try
            {
                SalveazaFacultati("C:\\Users\\victo\\Desktop\\ConsoleApp3\\facultati.txt");
                InregistreazaOperatie("Sa creat facultatea " + nume);
            }
            catch (IOException e)
            {
                Console.WriteLine("Eroare la salvarea facultatii: " + e.Message);
            }
        }
        public void AdaugaStudentLaFacultate(string numeFacultate, Student student)
        {
            if (facultati.ContainsKey(numeFacultate))
            {
                Facultate facultate = facultati[numeFacultate];
                string numeFisierStudenti = "C:\\Users\\victo\\Desktop\\ConsoleApp3\\studenti.txt";
                bool existaStudent = false;
                List<string> liniiActualizate = new List<string>();

                foreach (string linie in File.ReadLines(numeFisierStudenti))
                {
                    string[] partes = linie.Split(',');
                    if (partes.Length >= 3 && partes[0].Equals(student.Nume) && partes[1].Equals(student.Prenume) && partes[2].Equals(student.Email))
                    {
                        existaStudent = true;
                        partes[6] = numeFacultate; 
                        liniiActualizate.Add(string.Join(",", partes));
                    }
                    else
                    {
                        liniiActualizate.Add(linie);
                    }
                }

                if (!existaStudent)
                {
                    liniiActualizate.Add(student.ToFileLine(numeFacultate));
                }

                File.WriteAllLines(numeFisierStudenti, liniiActualizate);

                facultate.AdaugaStudent(student);

                InregistreazaOperatie("Atribuit studentul " + student.Nume + " la facultatea " + numeFacultate);
            }
            else
            {
                Console.WriteLine("Facultatea nu exista.");
            }
        }
        public void MarcheazaAbsolvireStudent(string numeFacultate, string numeStudent)
        {
            if (facultati.ContainsKey(numeFacultate))
            {
                Facultate facultate = facultati[numeFacultate];
                List<Student> studenti = facultate.GetStudenti();
                foreach (Student student in studenti)
                {
                    if (student.Nume.Equals(numeStudent))
                    {
                        student.SetAbsolvit(true);

                        string numeFisierStudenti = "C:\\Users\\victo\\Desktop\\ConsoleApp3\\studenti.txt";
                        string[] liniiStudenti = File.ReadAllLines(numeFisierStudenti);
                        List<string> liniiActualizate = new List<string>();

                        foreach (string linie in liniiStudenti)
                        {
                            string[] partes = linie.Split(',');
                            if (partes.Length >= 7 && partes[0].Equals(student.Nume) && partes[1].Equals(student.Prenume) && partes[6].Equals(numeFacultate))
                            { 
                                partes[5] = "True";
                                liniiActualizate.Add(string.Join(",", partes));
                                Console.WriteLine("Studentul a absolvit la facultatea");
                            }
                            else
                            {
                                liniiActualizate.Add(linie);
                            }
                        }
                        File.WriteAllLines(numeFisierStudenti, liniiActualizate);
                        InregistreazaOperatie("Studentul " + numeStudent + " a absolvit la facultatea " + numeFacultate);
                        return;
                    }
                }
                Console.WriteLine("Studentul nu a fost gasit in facultatea specificata.");
            }
            else
            {
                Console.WriteLine("Facultatea nu exista.");
            }
        }
        public void AfiseazaFacultati()
        {
            Console.WriteLine("Facultati:");
            foreach (string numeFacultate in facultati.Keys)
            {
                Console.WriteLine(numeFacultate);
            }
            InregistreazaOperatie("Afisat lista facultatilor");
        }
        
        public void AfiseazaFacultateDupaEmail(string emailStudent)
        {
            bool gasit = false;
            foreach (Facultate facultate in facultati.Values)
            {
                foreach (Student student in facultate.GetStudenti())
                {
                    if (student.Email.Equals(emailStudent))
                    {
                        Console.WriteLine("Studentul cu adresa de email " + emailStudent + " este absolvent al facultatii " + facultate.Nume);
                        gasit = true;
                        InregistreazaOperatie("Afisat facultatea pentru studentul cu adresa de email " + emailStudent);
                        break;
                    }
                }
                if (gasit)
                    break;
            }
            if (!gasit)
            {
                Console.WriteLine("Nu s-a gasit niciun student cu adresa de email " + emailStudent);
                InregistreazaOperatie("Cautat student cu adresa de email " + emailStudent + " (nu a fost gasit)");
            }
        }
        public void AfiseazaFacultatileDupaDomeniu(string domeniu)
        {
            Console.WriteLine($"Facultati din domeniul {domeniu}:");
            bool gasit = false;
            foreach (Facultate facultate in facultati.Values)
            {
                if (facultate.Domeniu.Equals(domeniu, StringComparison.OrdinalIgnoreCase))
                {
                    Console.Write("Numele facultatii: ");
                    Console.WriteLine(facultate.Nume);
                    gasit = true;
                    InregistreazaOperatie("Faculatatile au fost afisate dupa domeniul: " + domeniu);
                }
            }
            if (!gasit)
            {
                Console.WriteLine("Nu exista facultati în acest domeniu.");
            }
        }
        public void InregistreazaOperatie(string operatie)
        {
            try
            {
                logWriter.WriteLine( operatie + " pe data: " +DateTime.Now.ToString());
                logWriter.Flush();
            }
            catch (IOException e)
            {
                Console.WriteLine("Eroare la inregistrarea operatiei: " + e.Message);
            }
        }
        public void InchideLog()
        {
            try
            {
                logWriter.WriteLine("Esire din program pe data: " + DateTime.Now.ToString());
                logWriter.Close();
            }
            catch (IOException e)
            {
                Console.WriteLine("Eroare la inregistrarea operatiei de ieire: " + e.Message);
            }

        }
        public void SalveazaFacultati(string numeFisier)
        {
            using (StreamWriter writer = new StreamWriter(numeFisier))
            {
                foreach (Facultate facultate in facultati.Values)
                {
                    writer.WriteLine(facultate.Nume + "," + facultate.Abreviere + "," + facultate.Domeniu);
                }
            }
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            SistemManagementStudenti sistem = new SistemManagementStudenti();
            while (true)
            {
                Console.WriteLine("1. Atribuie un student la o facultate.");
                Console.WriteLine("2. Absolvire student.");  
                Console.WriteLine("3. Afiseaza stundeti inscrisi.");
                Console.WriteLine("4. Afiseaza stundeti absolviti.");
                Console.WriteLine("5. Afiseaza studenti.\n");
                Console.WriteLine("6. Creazti o facultate noua.");
                Console.WriteLine("7. Afiseaza facultatea la care este un absolvent dupa Email.");
                Console.WriteLine("8. Afiseaza facultatile.");
                Console.WriteLine("9. Afiseaza faculatile dupa un domeniu.");
                Console.WriteLine("0. Iesire");

                int alegere;
                if (!int.TryParse(Console.ReadLine(), out alegere))
                {
                    Console.WriteLine("Va rugam introduceti un numar intreg.");
                    continue;
                }

                switch (alegere)
                {
                    case 1:
                        Console.WriteLine("Introduceti numele facultatii:");
                        string? facultate = Console.ReadLine();

                        Console.WriteLine("Introduceti numele studentului:");
                        string? numeStudent = Console.ReadLine();

                        Console.WriteLine("Introduceti prenumele studentului:");
                        string? prenumeStudent = Console.ReadLine();

                        Console.WriteLine("Introduceti email-ul studentului:");
                        string? emailStudent = Console.ReadLine();

                        Console.WriteLine("Introduceti data nasterii a studentului (YYYY-MM-DD):");
                        DateTime dataNasterii;
                        if (!DateTime.TryParse(Console.ReadLine(), out dataNasterii))
                        {
                            Console.WriteLine("Format de data invalid. Utilizati formatul YYYY-MM-DD.");
                            continue;
                        }
                        Console.WriteLine("Introduceti data inscrierii a studentului (YYYY-MM-DD):"); 
                        
                        DateTime dataInscriere;
                        if (!DateTime.TryParse(Console.ReadLine(), out dataInscriere))
                        {
                            Console.WriteLine("Format de data invalid. Utilizati formatul YYYY-MM-DD.");
                            continue;
                        }
                        sistem.AdaugaStudentLaFacultate(facultate!, new Student(numeStudent!, prenumeStudent!, emailStudent!, dataNasterii, dataInscriere));
                        Console.WriteLine("Studentul a fost adagut cu succes la facultatea: " + facultate);
                        break;
                    case 2:
                        Console.WriteLine("Introduceti numele facultatii:");
                        string? facultateAbsolvire = Console.ReadLine();

                        Console.WriteLine("Introduceti numele studentului care absolveste:");
                        string? numeStudentAbsolvire = Console.ReadLine();

                        sistem.MarcheazaAbsolvireStudent(facultateAbsolvire!, numeStudentAbsolvire!);
                        break;
                    case 3:
                        Console.WriteLine("Introduceti numele facultatii:");
                        string? numeFacultateAfisare = Console.ReadLine();
                        if (sistem.facultati.ContainsKey(numeFacultateAfisare!))
                        {
                            sistem.AfiseazaStudentiNeabsolviti(numeFacultateAfisare!);
                        }
                        else
                        {
                            Console.WriteLine("Facultatea nu exista.");
                        }
                        break;
                    case 4:
                        Console.WriteLine("Introduceti numele facultatii:");
                        string? facultateAfisare = Console.ReadLine();
                        sistem.AfiseazaStudentiFacultate(facultateAfisare!);
                        break;

                    case 5:
                       Console.WriteLine("Lista studentilor:");
                        foreach (Facultate f in sistem.facultati.Values)
                        {
                            if (f.GetStudenti().Count > 0)
                            {
                                Console.WriteLine("Facultate: " + f.Nume);
                                Console.WriteLine("Studenti:");
                                foreach (Student student in f.GetStudenti())
                                {
                                    Console.WriteLine("Nume: " + student.Nume);
                                    Console.WriteLine("Prenume: " + student.Prenume);
                                    Console.WriteLine("Email: " + student.Email);
                                    Console.WriteLine("Data Nasterii: " + student.DataNasterii.ToString("yyyy-MM-dd"));
                                    Console.WriteLine("Data Inscrierii: " + student.DataInscriere.ToString("yyyy-MM-dd"));
                                    Console.WriteLine("Absolvit: " + (student.Absolvit ? "Da" : "Nu"));
                                    Console.WriteLine();
                                }
                                Console.WriteLine();
                            }
                        }
                        sistem.InregistreazaOperatie("Au fost afisati toti studenti");
                        break;
                    case 6:
                        Console.WriteLine("Introduceti numele facultatii:");
                        string? numeFacultate = Console.ReadLine();
                        Console.WriteLine("Introduceti abrevierea facultatii:");
                        string? abreviereFacultate = Console.ReadLine();
                        Console.WriteLine("Introduceti domeniu facultatii:");
                        string? domeniuFacultate = Console.ReadLine();
                        sistem.CreeazaFacultate(numeFacultate!, abreviereFacultate!, domeniuFacultate!);
                        break;
                    case 7:
                        Console.WriteLine("Introduceți adresa de email a studentului:");
                        string? emailStudentCautat = Console.ReadLine();
                        sistem.AfiseazaFacultateDupaEmail(emailStudentCautat!);
                        break;
                    case 8:
                        Console.WriteLine("Lista facultatilor:");
                        foreach (Facultate f in sistem.facultati.Values)
                        {
                            Console.WriteLine("Nume: " + f.Nume);
                            Console.WriteLine("Abreviere: " + f.Abreviere);
                            Console.WriteLine("Domeniu: " + f.Domeniu);
                            Console.WriteLine();
                        }
                        sistem.InregistreazaOperatie("Au fost afisate facultatile");
                        break;
                    case 9:
                        Console.WriteLine("Introduceti domeniul pentru care doriti sa afisati facultatile:");
                        string? domeniu = Console.ReadLine();
                        sistem.AfiseazaFacultatileDupaDomeniu(domeniu!);
                        break;
                    case 0:
                        sistem.InchideLog();
                        return;
                    default:
                        Console.WriteLine("Alegere invalida. Va rugam introduceti un numar intre 0 si 5.");
                        break;
                }
            }
        }
    }   
}
