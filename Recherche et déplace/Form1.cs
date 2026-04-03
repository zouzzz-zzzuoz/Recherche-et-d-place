namespace Recherche_et_déplace
{
    public partial class Form1 : Form
    {
        private Label versionLabel;
        private Button searchButton;
        private Label pathLabel;
        private ComboBox comboBoxColumn;
        private ComboBox comboBoxRow;
        private Label columnLabel;
        private Label rowLabel;
        private TextBox inputTextBox;
        private Button actionButton;

        public Form1()
        {
            InitializeComponent();
            // set version label text to format: par Romu vXXYYZZ-AA where XX=year, YY=month, ZZ=day, AA=daily build number
            var now = DateTime.Now;
            // XX year as two digits
            string xx = (now.Year % 100).ToString("D2");
            string yy = now.Month.ToString("D2");
            string zz = now.Day.ToString("D2");

            // determine daily build number AA from assembly version Build/Revision or fallback to 00
            string aa = "00";
            try
            {
                var asm = System.Reflection.Assembly.GetEntryAssembly() ?? System.Reflection.Assembly.GetExecutingAssembly();
                var ver = asm.GetName().Version;
                if (ver != null)
                {
                    // Use Revision as daily build number if present, otherwise Build
                    var buildNum = ver.Revision > 0 ? ver.Revision : ver.Build;
                    aa = (buildNum % 100).ToString("D2");
                }
            }
            catch
            {
                // ignore and keep default
            }

            versionLabel.Text = $"par Romu v{xx}{yy}{zz}-{aa}";
            // wire up events for controls (some handlers are attached also in designer for safety)
            searchButton.Click += SearchButton_Click;
            inputTextBox.TextChanged += InputTextBox_TextChanged;
            actionButton.Click += ActionButton_Click;
        }

        private void SearchButton_Click(object? sender, EventArgs e)
        {
            using var dlg = new OpenFileDialog();
            dlg.Filter = "Excel files|*.xlsx;*.xls;*.xlsm|All files|*.*";
            dlg.Title = "Sélectionner un fichier Excel";
            dlg.CheckFileExists = true;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                pathLabel.Text = dlg.FileName;
                UpdateActionButtonState();
            }
        }

        private void InputTextBox_TextChanged(object? sender, EventArgs e)
        {
            UpdateActionButtonState();
        }

        private void UpdateActionButtonState()
        {
            actionButton.Enabled = !string.IsNullOrWhiteSpace(pathLabel.Text) && !string.IsNullOrWhiteSpace(inputTextBox.Text);
        }

        private void ActionButton_Click(object? sender, EventArgs e)
        {
            var excelPath = pathLabel.Text;
            var folder = inputTextBox.Text;
            if (string.IsNullOrWhiteSpace(excelPath) || !File.Exists(excelPath))
            {
                MessageBox.Show("Fichier Excel introuvable.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(folder) || !Directory.Exists(folder))
            {
                MessageBox.Show("Dossier introuvable.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var column = comboBoxColumn.SelectedItem?.ToString() ?? "A";
            var startRowStr = comboBoxRow.SelectedItem?.ToString() ?? "1";
            if (!int.TryParse(startRowStr, out var startRow)) startRow = 1;

            int found = 0;
            try
            {
                var ext = Path.GetExtension(excelPath).ToLowerInvariant();
                if (ext == ".xlsx" || ext == ".xlsm")
                {
                    foreach (var value in ReadColumnValuesFromXlsx(excelPath, column, startRow))
                    {
                        if (string.IsNullOrWhiteSpace(value)) break;
                        if (IsFilePresentInFolder(folder, value)) found++;
                    }
                }
                else
                {
                    MessageBox.Show("Seuls les fichiers .xlsx et .xlsm sont supportés.", "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erreur lors de la lecture du fichier Excel :\n" + ex.Message, "Erreur", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show($"Fichiers trouvés : {found}", "Résultat", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private static bool IsFilePresentInFolder(string folder, string name)
        {
            name = name.Trim();
            if (string.IsNullOrEmpty(name)) return false;
            // If name has extension, check exact
            if (Path.HasExtension(name))
            {
                return File.Exists(Path.Combine(folder, name));
            }
            // otherwise check any file with this base name
            try
            {
                return Directory.EnumerateFiles(folder, name + ".*", SearchOption.TopDirectoryOnly).Any()
                    || File.Exists(Path.Combine(folder, name));
            }
            catch
            {
                return false;
            }
        }

        private static IEnumerable<string> ReadColumnValuesFromXlsx(string filePath, string columnLetter, int startRow)
        {
            // Simple .xlsx reader: parse sharedStrings and sheet1.xml inside the zip
            var results = new Dictionary<int, string>();
            using var archive = System.IO.Compression.ZipFile.OpenRead(filePath);
            // read shared strings
            var shared = new List<string>();
            var sse = archive.GetEntry("xl/sharedStrings.xml");
            if (sse != null)
            {
                using var ssStream = sse.Open();
                var doc = new System.Xml.XmlDocument();
                doc.Load(ssStream);
                var nsmgr = new System.Xml.XmlNamespaceManager(doc.NameTable);
                nsmgr.AddNamespace("d", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
                var siNodes = doc.SelectNodes("//d:si", nsmgr);
                if (siNodes != null)
                {
                    foreach (System.Xml.XmlNode si in siNodes)
                    {
                        // concatenate text inside t nodes
                        var tNodes = si.SelectNodes(".//d:t", nsmgr);
                        if (tNodes != null)
                        {
                            var sb = new System.Text.StringBuilder();
                            foreach (System.Xml.XmlNode t in tNodes) sb.Append(t.InnerText);
                            shared.Add(sb.ToString());
                        }
                        else
                        {
                            shared.Add(string.Empty);
                        }
                    }
                }
            }

            // read first worksheet (sheet1.xml)
            var sheetEntry = archive.GetEntry("xl/worksheets/sheet1.xml");
            if (sheetEntry == null)
            {
                // try to find any sheet file
                sheetEntry = archive.Entries.FirstOrDefault(e => e.FullName.StartsWith("xl/worksheets/sheet", StringComparison.OrdinalIgnoreCase));
            }
            if (sheetEntry == null) yield break;

            using var sheetStream = sheetEntry.Open();
            var xdoc = new System.Xml.XmlDocument();
            xdoc.Load(sheetStream);
            var mgr = new System.Xml.XmlNamespaceManager(xdoc.NameTable);
            mgr.AddNamespace("d", "http://schemas.openxmlformats.org/spreadsheetml/2006/main");
            var cellNodes = xdoc.SelectNodes("//d:c", mgr);
            if (cellNodes != null)
            {
                foreach (System.Xml.XmlNode c in cellNodes)
                {
                    var r = c.Attributes?["r"]?.Value; // like D3
                    if (string.IsNullOrEmpty(r)) continue;
                    // check column
                    var colPart = new string(r.TakeWhile(ch => !char.IsDigit(ch)).ToArray());
                    var rowPart = new string(r.SkipWhile(ch => !char.IsDigit(ch)).ToArray());
                    if (!string.Equals(colPart, columnLetter, StringComparison.OrdinalIgnoreCase)) continue;
                    if (!int.TryParse(rowPart, out var rowNum)) continue;
                    string val = string.Empty;
                    var tAttr = c.Attributes?["t"]?.Value;
                    var vNode = c.SelectSingleNode("d:v", mgr);
                    if (vNode != null)
                    {
                        val = vNode.InnerText;
                        if (tAttr == "s")
                        {
                            if (int.TryParse(val, out var idx) && idx >= 0 && idx < shared.Count) val = shared[idx];
                        }
                    }
                    else
                    {
                        // inline string
                        var isNode = c.SelectSingleNode("d:is", mgr);
                        if (isNode != null)
                        {
                            var tNode = isNode.SelectSingleNode(".//d:t", mgr);
                            if (tNode != null) val = tNode.InnerText;
                        }
                    }

                    results[rowNum] = val;
                }
            }

            // iterate from startRow until missing or empty
            for (int r = startRow; ; r++)
            {
                if (!results.TryGetValue(r, out var v) || string.IsNullOrWhiteSpace(v)) yield break;
                yield return v;
            }
        }
    }
}
