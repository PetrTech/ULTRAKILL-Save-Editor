using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ULTRAKILL_Save_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public string saveFolder = "";
        SaveSelect selectWindow;
        byte[] globalSave;
        byte[] levelSave;
        int secrets;
        GlobalSave gSave;

        public MainWindow()
        {
            InitializeComponent();
        }

        public void SelectSave(string folder)
        {
            saveFolder = folder;
            Debug.WriteLine(saveFolder);
            interactBlock.Visibility = Visibility.Collapsed;
            LoadSave();
            ReadLevel();
        }

        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            interactBlock.Visibility = Visibility.Visible;
            selectWindow = new();
            selectWindow.CreateWindow(this);
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            interactBlock.Visibility = Visibility.Visible;
            selectWindow.Show();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            // Create backup
            ZipFile.CreateFromDirectory(saveFolder, "UK-bk" + DateTime.Now.ToString("MMddyyyyhmmsstt") + ".zip");
            Save();
        }

        public int HexToDecimal(byte[] data)
        {
            return BitConverter.ToInt32(data, 0);
        }

        public bool HexToBool(byte value) {

            return value != 0x00;
        }

        public int HexToComboVal(byte value)
        {
            if (value == 0x00) return 0;
            else if (value == 0x01) return 1;
            else if (value == 0x02) return 2;
            else return 0;
        }

        public byte[] Extract(byte[] array, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            byte[] ret = new byte[length];
            Array.Copy(array, startIndex, ret, 0, length);
            return ret;
        }

        public byte Extract(byte[] array, int index)
        {
            return array[index];
        }

        public void LoadSave()
        {
            // load global
            globalSave = File.ReadAllBytes(System.IO.Path.Combine(saveFolder, "generalprogress.bepis"));

            // load stats
            gSave = new();

            // load money
            byte[] moneyExtract = Extract(globalSave, 0x25f, 0x262 + 0x001);
            int money = HexToDecimal(moneyExtract);
            pts.Value = money;

            // load progression
            byte[] progressionExtract = Extract(globalSave, 0x263, 0x265 + 0x001);
            bool seenIntro = HexToBool(progressionExtract[0]);
            bool beatTutorial = HexToBool(progressionExtract[1]);
            bool unlockedClashMode = HexToBool(progressionExtract[2]);
            introSeen.IsChecked = seenIntro;
            tutorialBeat.IsChecked = beatTutorial;
            clashModeUnlocked.IsChecked = unlockedClashMode;

            // load weapon progression
            // REVOLVER
            byte rev0 = Extract(globalSave, 0x266);
            byte rev1 = Extract(globalSave, 0x26A);
            byte rev2 = Extract(globalSave, 0x26E);
            byte revalt = Extract(globalSave, 0x276);

            revolverblue.IsChecked = HexToBool(rev0);
            revolverred.IsChecked = HexToBool(rev1);
            revolvergreen.IsChecked = HexToBool(rev2);
            revolveralt.IsChecked = HexToBool(revalt);

            // SHOTGUN
            byte sho0 = Extract(globalSave, 0x27A);
            byte sho1 = Extract(globalSave, 0x27E);

            shotgunblue.IsChecked = HexToBool(sho0);
            shotgungreen.IsChecked = HexToBool(sho1);

            // NAILGUN
            byte nai0 = Extract(globalSave, 0x28A);
            byte nai1 = Extract(globalSave, 0x28E);
            byte naialt = Extract(globalSave, 0x29A);

            nailgunblue.IsChecked = HexToBool(nai0);
            nailgungreen.IsChecked = HexToBool(nai1);
            nailgunalt.IsChecked = HexToBool(naialt);

            // RAILCANNON
            byte rai0 = Extract(globalSave, 0x29E);
            byte rai1 = Extract(globalSave, 0x2A2);
            byte rai2 = Extract(globalSave, 0x2A6);

            railcannonblue.IsChecked = HexToBool(rai0);
            railcannongreen.IsChecked = HexToBool(rai1);
            railcannonred.IsChecked = HexToBool(rai2);

            // ROCKET LAUNCHER
            byte rock0 = Extract(globalSave, 0x2AE);
            byte rock1 = Extract(globalSave, 0x2B2);

            rocketblue.IsChecked = HexToBool(rock0);
            rocketgreen.IsChecked = HexToBool(rock1);

            // ARMS
            byte arm1 = Extract(globalSave, 0x2CE);
            byte arm2 = Extract(globalSave, 0x2D2);

            armred.IsChecked = HexToBool(arm1);
            armgreen.IsChecked = HexToBool(arm2);

            // load secret missions
            byte mis0 = Extract(globalSave, 0x2FD);
            byte mis1 = Extract(globalSave, 0x301);
            byte mis2 = Extract(globalSave, 0x305);
            byte mis3 = Extract(globalSave, 0x30D);
            byte mis4 = Extract(globalSave, 0x311);

            secmis0.SelectedIndex = HexToComboVal(mis0);
            secmis1.SelectedIndex = HexToComboVal(mis1);
            secmis2.SelectedIndex = HexToComboVal(mis2);
            secmis3.SelectedIndex = HexToComboVal(mis3);
            secmis4.SelectedIndex = HexToComboVal(mis4);

            // load customization
            byte revcustom = Extract(globalSave, 0x2EE);
            byte shocustom = Extract(globalSave, 0x2EF);
            byte naicustom = Extract(globalSave, 0x2F0);
            byte raicustom = Extract(globalSave, 0x2F1);
            byte rockcustom = Extract(globalSave, 0x2F2);

            revcustomization.IsChecked = HexToBool(revcustom);
            shocustomization.IsChecked = HexToBool(shocustom);
            naicustomization.IsChecked = HexToBool(naicustom);
            raicustomization.IsChecked = HexToBool(raicustom);
            rockcustomization.IsChecked = HexToBool(rockcustom);
        }

        public void Save()
        {
            gSave.money = (int)pts.Value;

            gSave.seenIntro = (bool)introSeen.IsChecked;
            gSave.beatTutorial = (bool)tutorialBeat.IsChecked;
            gSave.unlockedClashMode = (bool)clashModeUnlocked.IsChecked;

            gSave.rev0 = (bool)revolverblue.IsChecked;
            gSave.rev1 = (bool)revolvergreen.IsChecked;
            gSave.rev2 = (bool)revolverred.IsChecked;
            gSave.revalt = (bool)revolveralt.IsChecked;

            gSave.sho0 = (bool)shotgunblue.IsChecked;
            gSave.sho1 = (bool)shotgungreen.IsChecked;

            gSave.nai0 = (bool)nailgunblue.IsChecked;
            gSave.nai1 = (bool)nailgungreen.IsChecked;
            gSave.naialt = (bool)nailgunalt.IsChecked;

            gSave.rai0 = (bool)railcannonblue.IsChecked;
            gSave.rai1 = (bool)railcannongreen.IsChecked;
            gSave.rai2 = (bool)railcannonred.IsChecked;

            gSave.rock0 = (bool)rocketblue.IsChecked;
            gSave.rock1 = (bool)rocketgreen.IsChecked;

            gSave.arm1 = (bool)armred.IsChecked;
            gSave.arm2 = (bool)armgreen.IsChecked;

            gSave.mis0 = secmis0.SelectedIndex;
            gSave.mis1 = secmis1.SelectedIndex;
            gSave.mis2 = secmis2.SelectedIndex;
            gSave.mis3 = secmis3.SelectedIndex;
            gSave.mis4 = secmis4.SelectedIndex;

            gSave.customrev = (bool)revcustomization.IsChecked;
            gSave.customsho = (bool)shocustomization.IsChecked;
            gSave.customnai = (bool)naicustomization.IsChecked;
            gSave.customrai = (bool)raicustomization.IsChecked;
            gSave.customrock = (bool)rockcustomization.IsChecked;

            try
            {
                gSave.SaveFile(globalSave, saveFolder);
                MessageBox.Show("Save successful!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving the file: " + ex.Message);
            }
        }

        public int GetIndexFromByte(byte value)
        {
            switch (value)
            {
                case 0xFF:
                    return 0;
                case 0x00:
                    return 1;
                case 0x01:
                    return 2;
                case 0x02:
                    return 3;
                case 0x03:
                    return 4;
                case 0x04:
                    return 5;
                case 0x0C:
                    return 6;
                default:
                    return 0;
            }
        }

        public int GetSecretsFromByte(byte value)
        {
            switch (value)
            {
                case 0x00:
                    return 0;
                case 0x01:
                    return 1;
                case 0x02:
                    return 2;
                case 0x03:
                    return 3;
                case 0x04:
                    return 4;
                case 0x05:
                    return 5;
                default:
                    return 0;
            }
        }

        private void NumericUpDown_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            ReadLevel();
        }

        public void ReadLevel()
        {
            if (nounlock == null) return;

            // REFRESH LEVEL PAGE
            string levelPath = System.IO.Path.Combine(saveFolder, "lvl" + lvl.Value + "progress.bepis");
            if (File.Exists(levelPath))
            {
                nounlock.Visibility = Visibility.Collapsed;

                // READ FILE
                levelSave = File.ReadAllBytes(levelPath);

                // READ RANKS
                byte harmlessrank = levelSave[0x0E5];
                byte lenientrank = levelSave[0x0E9];
                byte standardrank = levelSave[0x0ED];
                byte violentrank = levelSave[0x0F1];

                harmlessRank.SelectedIndex = GetIndexFromByte(harmlessrank);
                lenientRank.SelectedIndex = GetIndexFromByte(lenientrank);
                standardRank.SelectedIndex = GetIndexFromByte(standardrank);
                violentRank.SelectedIndex = GetIndexFromByte(violentrank);

                // READ SECRETS
                byte secretsAmount = levelSave[0x0C8];
                secrets = GetSecretsFromByte(secretsAmount);

                sec5.IsEnabled = true;
                sec4.IsEnabled = true;
                sec3.IsEnabled = true;
                sec2.IsEnabled = true;
                sec1.IsEnabled = true;

                sec5.IsChecked = false;
                sec4.IsChecked = false;
                sec3.IsChecked = false;
                sec2.IsChecked = false;
                sec1.IsChecked = false;

                switch (secrets)
                {
                    case 4:
                        sec5.IsEnabled = false;
                        break;
                    case 3:
                        sec5.IsEnabled = false;
                        sec4.IsEnabled = false;
                        break;
                    case 2:
                        sec5.IsEnabled = false;
                        sec4.IsEnabled = false;
                        sec3.IsEnabled = false;
                        break;
                    case 1:
                        sec5.IsEnabled = false;
                        sec4.IsEnabled = false;
                        sec3.IsEnabled = false;
                        sec2.IsEnabled = false;
                        break;
                    case 0:
                        sec5.IsEnabled = false;
                        sec4.IsEnabled = false;
                        sec3.IsEnabled = false;
                        sec2.IsEnabled = false;
                        sec1.IsEnabled = false;
                        break;
                    default:break;
                }

                byte[] secretsFound = new byte[secrets];
                Array.Copy(levelSave, 0x107, secretsFound, 0, secrets);

                if (secrets >= 1) sec1.IsChecked = HexToBool(secretsFound[0]);
                if (secrets >= 2) sec2.IsChecked = HexToBool(secretsFound[1]);
                if (secrets >= 3) sec3.IsChecked = HexToBool(secretsFound[2]);
                if (secrets >= 4) sec4.IsChecked = HexToBool(secretsFound[3]);
                if (secrets >= 5) sec5.IsChecked = HexToBool(secretsFound[4]);

                // challenge
                byte isDone = levelSave[0x0D1];
                challengeDone.IsOn = HexToBool(isDone);

                // major assists
                byte majorAssist = levelSave[0x116];
                majorAssists.IsOn = HexToBool(majorAssist);
            }
            else
            {
                nounlock.Visibility = Visibility.Visible;
            }
        }

        public byte[] rankToByte(int rank)
        {
            switch (rank)
            {
                case 0:
                    return new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
                case 1:
                    return new byte[] { 0x00, 0x00, 0x00, 0x00 };
                case 2:
                    return new byte[] { 0x01, 0x00, 0x00, 0x00 };
                case 3:
                    return new byte[] { 0x02, 0x00, 0x00, 0x00 };
                case 4:
                    return new byte[] { 0x03, 0x00, 0x00, 0x00 };
                case 5:
                    return new byte[] { 0x04, 0x00, 0x00, 0x00 };
                case 6:
                    return new byte[] { 0x0C, 0x00, 0x00, 0x00 };
                default: return new byte[] { 0xFF, 0xFF, 0xFF, 0xFF };
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string levelPath = System.IO.Path.Combine(saveFolder, "lvl" + lvl.Value + "progress.bepis");

            // SAVE RANKS
            byte[] levelSaveCopy = gSave.OverwriteDataChunk(levelSave, rankToByte(harmlessRank.SelectedIndex), 0x0E5);
            levelSaveCopy = gSave.OverwriteDataChunk(levelSaveCopy, rankToByte(lenientRank.SelectedIndex), 0x0E9);
            levelSaveCopy = gSave.OverwriteDataChunk(levelSaveCopy, rankToByte(standardRank.SelectedIndex), 0x0ED);
            levelSaveCopy = gSave.OverwriteDataChunk(levelSaveCopy, rankToByte(violentRank.SelectedIndex), 0x0F1);

            // SAVE SECRETS
            if (secrets != 0)
            {
                byte[] foundSecrets = new byte[secrets];

                if(secrets >= 1)
                {
                    foundSecrets[0] = Convert.ToByte(sec1.IsChecked);
                }
                if (secrets >= 2)
                {
                    foundSecrets[1] = Convert.ToByte(sec2.IsChecked);
                }
                if (secrets >= 3)
                {
                    foundSecrets[2] = Convert.ToByte(sec3.IsChecked);
                }
                if (secrets >= 4)
                {
                    foundSecrets[3] = Convert.ToByte(sec4.IsChecked);
                }
                if (secrets == 5)
                {
                    foundSecrets[4] = Convert.ToByte(sec5.IsChecked);
                }

                levelSaveCopy = gSave.OverwriteDataChunk(levelSaveCopy, foundSecrets, 0x107);
            }

            byte isChallengeDone = Convert.ToByte(challengeDone.IsOn);
            levelSaveCopy = gSave.OverwriteByte(levelSaveCopy, isChallengeDone, 0x0D1);

            byte majorAssistsUsed = Convert.ToByte(majorAssists.IsOn);
            levelSaveCopy = gSave.OverwriteDataChunk(levelSaveCopy, new byte[] { majorAssistsUsed, majorAssistsUsed, majorAssistsUsed, majorAssistsUsed, majorAssistsUsed, majorAssistsUsed }, 0x116);

            File.WriteAllBytes(levelPath, levelSaveCopy);
        }
    }

    public class GlobalSave
    {
        public int money;
        public bool seenIntro, beatTutorial, unlockedClashMode;

        public bool rev0,rev1,rev2,revalt;
        public bool sho0, sho1;
        public bool nai0, nai1, naialt;
        public bool rai0, rai1, rai2;
        public bool rock0, rock1;
        public bool arm1, arm2;

        public int mis0, mis1, mis2, mis3, mis4;

        public bool customrev, customsho, customnai, customrai, customrock;

        // used to write multiple bytes
        public byte[] OverwriteDataChunk(byte[] array, byte[] data, int startIndex)
        {
            byte[] arrayCopy = new byte[array.Length];
            Array.Copy(array, arrayCopy, array.Length);

            for (int i = 0; i < data.Length; i++)
            {
                arrayCopy[i + startIndex] = data[i];
            }

            return arrayCopy;
        }

        // used to write ONE byte, esp. useful for booleans
        public byte[] OverwriteByte(byte[] array, byte data, int index)
        {
            byte[] arrayCopy = new byte[array.Length];
            array.CopyTo(arrayCopy, 0);

            arrayCopy[index] = data;

            return arrayCopy;
        }

        public byte ComboValToByte(int val)
        {
            if (val == 0) return 0x00;
            else if (val == 1) return 0x01;
            else if (val == 2) return 0x02;
            else return 0x00;
        }

        public void SaveFile(byte[] array, string dir)
        {
            // overwrite money
            byte[] arrayCopy = OverwriteDataChunk(array, BitConverter.GetBytes(money), 0x25f);
            Debug.WriteLine(money);

            // overwrite progression
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(seenIntro), 0x263);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(beatTutorial), 0x264);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(unlockedClashMode), 0x265);

            // overwrite weapon progression
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rev0), 0x266);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rev1), 0x26A);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rev2), 0x26E);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(revalt), 0x276);

            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(sho0), 0x27A);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(sho1), 0x27E);

            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(nai0), 0x28A);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(nai1), 0x28E);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(naialt), 0x29A);

            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rai0), 0x29E);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rai1), 0x2A2);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rai2), 0x2A6);

            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rock0), 0x2AE);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(rock1), 0x2B2);

            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(arm1), 0x2CE);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(arm2), 0x2D2);

            // overwrite secret missions
            arrayCopy = OverwriteByte(arrayCopy, ComboValToByte(mis0), 0x2FD);
            arrayCopy = OverwriteByte(arrayCopy, ComboValToByte(mis1), 0x301);
            arrayCopy = OverwriteByte(arrayCopy, ComboValToByte(mis2), 0x305);
            arrayCopy = OverwriteByte(arrayCopy, ComboValToByte(mis3), 0x30D);
            arrayCopy = OverwriteByte(arrayCopy, ComboValToByte(mis4), 0x311);

            // overwrite customization
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(customrev), 0x2EE);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(customsho), 0x2EF);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(customnai), 0x2F0);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(customrai), 0x2F1);
            arrayCopy = OverwriteByte(arrayCopy, Convert.ToByte(customrock), 0x2F2);

            File.WriteAllBytes(System.IO.Path.Combine(dir, "generalprogress.bepis"), arrayCopy);
        }
    }
}
