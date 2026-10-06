using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.SceneManagement;
using TMPro;
using Chromalit.UI;
using Chromalit.Interactables;

namespace Chromalit.EditorTools
{
    public static class HintUISetup
    {
        private const string ScrollSpritePath = "Assets/_Chromalit/Sprites/Environment/hint.png";
        private const string PrefabSavePath = "Assets/_Chromalit/Prefabs/Environment/MapSign.prefab";
        private const string ParchmentSpritePath = "Assets/_Chromalit/Sprites/UI/parchment_panel.png";

        static readonly UnityEngine.Color Parchment = new UnityEngine.Color(0.96f, 0.89f, 0.76f);       // #F5E3C2
        static readonly UnityEngine.Color BorderDark = new UnityEngine.Color(0.28f, 0.20f, 0.14f);       // #473324
        static readonly UnityEngine.Color BorderMid = new UnityEngine.Color(0.55f, 0.40f, 0.28f);        // #8C6647
        static readonly UnityEngine.Color Ornament = new UnityEngine.Color(0.42f, 0.35f, 0.55f);         // #6B598C
        static readonly UnityEngine.Color OrnamentDot = new UnityEngine.Color(0.55f, 0.47f, 0.65f);      // #8C78A6
        static readonly UnityEngine.Color TitleColor = new UnityEngine.Color(0.36f, 0.23f, 0.16f);       // #5B3A29
        static readonly UnityEngine.Color BodyColor = new UnityEngine.Color(0.36f, 0.23f, 0.16f);
        static readonly UnityEngine.Color CloseColor = new UnityEngine.Color(0.55f, 0.42f, 0.35f);       // #8B6B5A

        [MenuItem("Chromalit/Hint/Setup Hint UI In Scene")]
        private static void SetupHintUI()
        {
            Canvas canvas = null;
            GameObject hudGo = GameObject.Find("HUDCanvas");
            if (hudGo != null)
                canvas = hudGo.GetComponent<Canvas>();

            if (canvas == null)
            {
                hudGo = new GameObject("HUDCanvas");
                Undo.RegisterCreatedObjectUndo(hudGo, "Create HUDCanvas");
                canvas = hudGo.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;
                canvas.sortingOrder = 100;

                var scaler = hudGo.AddComponent<CanvasScaler>();
                scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);
                scaler.matchWidthOrHeight = 0.5f;

                hudGo.AddComponent<GraphicRaycaster>();
            }

            // --- HintOverlay (overlay gelap fullscreen) ---
            var overlayGo = FindOrCreateChild(hudGo, "HintOverlay");
            SetupImage(overlayGo, new UnityEngine.Color(0f, 0f, 0f, 0.59f), true);
            StretchFull(overlayGo);

            // --- Scroll (container tengah, tidak ada image sendiri) ---
            var scrollGo = FindOrCreateChild(overlayGo, "Scroll");
            var scrollRect = GetOrAddRect(scrollGo);
            SetAnchorCenter(scrollRect);
            scrollRect.sizeDelta = new Vector2(620, 480);
            scrollRect.anchoredPosition = Vector2.zero;

            // --- Pixel parchment panel dibangun dari UI layers ---

            // Border luar (gelap)
            var borderOuter = FindOrCreateChild(scrollGo, "BorderOuter");
            SetupImage(borderOuter, BorderDark);
            StretchFull(borderOuter);

            // Border tengah (mid brown), inset 4px
            var borderMid = FindOrCreateChild(borderOuter, "BorderMid");
            SetupImage(borderMid, BorderMid);
            StretchWithPadding(borderMid, 4);

            // Body parchment, inset 4px lagi (total 8px border)
            var bodyBg = FindOrCreateChild(borderMid, "ParchmentBody");
            SetupImage(bodyBg, Parchment);
            StretchWithPadding(bodyBg, 4);

            // Inner border line (1px gelap di dalam parchment, 6px dari edge parchment)
            var innerLine = FindOrCreateChild(bodyBg, "InnerLine");
            SetupImage(innerLine, BorderMid, 0.4f);
            StretchWithPadding(innerLine, 12);

            // Inner fill (parchment lagi, 1px inset dari inner line)
            var innerFill = FindOrCreateChild(innerLine, "InnerFill");
            SetupImage(innerFill, Parchment);
            StretchWithPadding(innerFill, 2);

            // --- Ornamen atas (garis + diamond tengah) ---
            var ornamentBar = FindOrCreateChild(bodyBg, "OrnamentBar");
            GetOrAddRect(ornamentBar);
            var ornBarRect = ornamentBar.GetComponent<RectTransform>();
            ornBarRect.anchorMin = new Vector2(0.5f, 1f);
            ornBarRect.anchorMax = new Vector2(0.5f, 1f);
            ornBarRect.pivot = new Vector2(0.5f, 1f);
            ornBarRect.anchoredPosition = new Vector2(0, -28);
            ornBarRect.sizeDelta = new Vector2(300, 20);

            // Garis horizontal ornamen
            var ornLine = FindOrCreateChild(ornamentBar, "OrnLine");
            SetupImage(ornLine, OrnamentDot, 0.6f);
            var ornLineRect = ornLine.GetComponent<RectTransform>();
            ornLineRect.anchorMin = new Vector2(0f, 0.5f);
            ornLineRect.anchorMax = new Vector2(1f, 0.5f);
            ornLineRect.pivot = new Vector2(0.5f, 0.5f);
            ornLineRect.offsetMin = new Vector2(20, -1);
            ornLineRect.offsetMax = new Vector2(-20, 1);

            // Diamond tengah
            var diamond = FindOrCreateChild(ornamentBar, "Diamond");
            SetupImage(diamond, Ornament);
            var diamondRect = diamond.GetComponent<RectTransform>();
            SetAnchorCenter(diamondRect);
            diamondRect.sizeDelta = new Vector2(16, 16);
            diamondRect.anchoredPosition = Vector2.zero;
            diamondRect.localRotation = Quaternion.Euler(0, 0, 45);

            // Dots kiri & kanan
            CreateDot(ornamentBar, "DotL1", -50, OrnamentDot);
            CreateDot(ornamentBar, "DotL2", -70, OrnamentDot);
            CreateDot(ornamentBar, "DotL3", -90, OrnamentDot);
            CreateDot(ornamentBar, "DotR1", 50, OrnamentDot);
            CreateDot(ornamentBar, "DotR2", 70, OrnamentDot);
            CreateDot(ornamentBar, "DotR3", 90, OrnamentDot);

            // --- Corner dots (4 sudut) ---
            CreateCornerDot(bodyBg, "CornerTL", new Vector2(0f, 1f), new Vector2(18, -18));
            CreateCornerDot(bodyBg, "CornerTR", new Vector2(1f, 1f), new Vector2(-18, -18));
            CreateCornerDot(bodyBg, "CornerBL", new Vector2(0f, 0f), new Vector2(18, 18));
            CreateCornerDot(bodyBg, "CornerBR", new Vector2(1f, 0f), new Vector2(-18, 18));

            // --- ContentArea (RectMask2D untuk clip teks) ---
            var contentGo = FindOrCreateChild(bodyBg, "ContentArea");
            GetOrAddRect(contentGo);
            if (contentGo.GetComponent<RectMask2D>() == null)
                contentGo.AddComponent<RectMask2D>();
            var contentRect = contentGo.GetComponent<RectTransform>();
            contentRect.anchorMin = Vector2.zero;
            contentRect.anchorMax = Vector2.one;
            contentRect.offsetMin = new Vector2(32, 24);
            contentRect.offsetMax = new Vector2(-32, -52);

            // --- TitleText ---
            var titleTMP = FindOrCreateTMP(contentGo.transform, "TitleText", "Create TitleText");
            var titleRect = titleTMP.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.anchoredPosition = new Vector2(0, -8);
            titleRect.sizeDelta = new Vector2(0, 50);
            titleTMP.fontSize = 32;
            titleTMP.fontStyle = FontStyles.Bold;
            titleTMP.alignment = TextAlignmentOptions.Center;
            titleTMP.color = TitleColor;
            titleTMP.textWrappingMode = TextWrappingModes.NoWrap;
            titleTMP.text = "Judul Petunjuk";

            // --- Separator line di bawah title ---
            var sepLine = FindOrCreateChild(contentGo, "Separator");
            SetupImage(sepLine, BorderMid, 0.3f);
            var sepRect = sepLine.GetComponent<RectTransform>();
            sepRect.anchorMin = new Vector2(0.15f, 1f);
            sepRect.anchorMax = new Vector2(0.85f, 1f);
            sepRect.pivot = new Vector2(0.5f, 1f);
            sepRect.anchoredPosition = new Vector2(0, -60);
            sepRect.sizeDelta = new Vector2(0, 2);

            // --- BodyText ---
            var bodyTMP = FindOrCreateTMP(contentGo.transform, "BodyText", "Create BodyText");
            var bodyRect = bodyTMP.GetComponent<RectTransform>();
            bodyRect.anchorMin = new Vector2(0f, 0.15f);
            bodyRect.anchorMax = new Vector2(1f, 0.82f);
            bodyRect.pivot = new Vector2(0.5f, 0.5f);
            bodyRect.offsetMin = new Vector2(10, 0);
            bodyRect.offsetMax = new Vector2(-10, 0);
            bodyTMP.fontSize = 22;
            bodyTMP.alignment = TextAlignmentOptions.Center;
            bodyTMP.textWrappingMode = TextWrappingModes.Normal;
            bodyTMP.color = BodyColor;
            bodyTMP.text = "Isi petunjuk di sini.";

            // --- CloseHintText ---
            var closeTMP = FindOrCreateTMP(contentGo.transform, "CloseHintText", "Create CloseHintText");
            var closeRect = closeTMP.GetComponent<RectTransform>();
            closeRect.anchorMin = new Vector2(0f, 0f);
            closeRect.anchorMax = new Vector2(1f, 0f);
            closeRect.pivot = new Vector2(0.5f, 0f);
            closeRect.anchoredPosition = new Vector2(0, 4);
            closeRect.sizeDelta = new Vector2(0, 36);
            closeTMP.fontSize = 16;
            closeTMP.alignment = TextAlignmentOptions.Center;
            closeTMP.color = CloseColor;
            closeTMP.textWrappingMode = TextWrappingModes.NoWrap;
            closeTMP.text = "[ E ] Tutup";

            // --- HintPanelManager ---
            GameObject managerGo = GameObject.Find("HintPanelManager");
            if (managerGo == null)
            {
                managerGo = new GameObject("HintPanelManager");
                Undo.RegisterCreatedObjectUndo(managerGo, "Create HintPanelManager");
            }
            var panelUI = managerGo.GetComponent<HintPanelUI>();
            if (panelUI == null) panelUI = managerGo.AddComponent<HintPanelUI>();

            var so = new SerializedObject(panelUI);
            so.FindProperty("panel").objectReferenceValue = overlayGo;
            so.FindProperty("scroll").objectReferenceValue = scrollGo.GetComponent<RectTransform>();
            so.FindProperty("titleText").objectReferenceValue = titleTMP;
            so.FindProperty("bodyText").objectReferenceValue = bodyTMP;
            so.FindProperty("closeHintText").objectReferenceValue = closeTMP;
            so.ApplyModifiedProperties();

            overlayGo.SetActive(false);

            EditorSceneManager.MarkSceneDirty(UnityEngine.SceneManagement.SceneManager.GetActiveScene());
            Debug.Log("[HintUISetup] Hint UI (parchment panel) berhasil dibuat di scene.");
        }

        [MenuItem("Chromalit/Hint/Create MapSign Prefab")]
        private static void CreateMapSignPrefab()
        {
            Sprite scrollSprite = AssetDatabase.LoadAssetAtPath<Sprite>(ScrollSpritePath);
            if (scrollSprite == null)
                Debug.LogError($"[HintUISetup] Sprite tidak ditemukan: {ScrollSpritePath}");

            GameObject mapSign = new GameObject("MapSign");

            var sr = mapSign.AddComponent<SpriteRenderer>();
            sr.sortingOrder = 1;
            if (scrollSprite != null) sr.sprite = scrollSprite;
            mapSign.transform.localScale = Vector3.one * 0.4f;

            var hintSign = mapSign.AddComponent<HintSign>();

            string[] guids = AssetDatabase.FindAssets("InteractPrompt t:Prefab");
            GameObject promptInstance = null;
            if (guids.Length > 0)
            {
                string prefabPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                GameObject promptPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                promptInstance = (GameObject)PrefabUtility.InstantiatePrefab(promptPrefab);
                promptInstance.transform.SetParent(mapSign.transform, false);
                promptInstance.transform.localPosition = new Vector3(0, 1.5f, 0);
                promptInstance.transform.localScale = Vector3.one * 0.015f;
            }
            else
            {
                Debug.LogError("[HintUISetup] Prefab InteractPrompt tidak ditemukan di project!");
            }

            if (promptInstance != null)
            {
                var promptComp = promptInstance.GetComponent<InteractPrompt>();
                if (promptComp != null)
                {
                    var so = new SerializedObject(hintSign);
                    so.FindProperty("prompt").objectReferenceValue = promptComp;
                    so.ApplyModifiedProperties();
                }
            }

            string dir = System.IO.Path.GetDirectoryName(PrefabSavePath);
            if (!AssetDatabase.IsValidFolder(dir))
                AssetDatabase.CreateFolder("Assets/_Chromalit/Prefabs", "Environment");

            PrefabUtility.SaveAsPrefabAsset(mapSign, PrefabSavePath);
            Object.DestroyImmediate(mapSign);

            Debug.Log($"[HintUISetup] MapSign prefab disimpan di {PrefabSavePath}");
        }

        [MenuItem("Chromalit/Hint/Fix hint_scroll Import Settings")]
        private static void FixHintScrollImport()
        {
            var importer = AssetImporter.GetAtPath(ScrollSpritePath) as TextureImporter;
            if (importer == null)
            {
                Debug.LogError($"[HintUISetup] TextureImporter tidak ditemukan: {ScrollSpritePath}");
                return;
            }

            importer.textureType = TextureImporterType.Sprite;
            importer.spriteImportMode = SpriteImportMode.Single;
            importer.filterMode = FilterMode.Point;
            importer.textureCompression = TextureImporterCompression.Uncompressed;
            importer.mipmapEnabled = false;
            importer.alphaIsTransparency = true;
            importer.SaveAndReimport();

            Debug.Log("[HintUISetup] Import settings hint.png sudah diperbaiki.");
        }

        // ====== Helper methods ======

        private static GameObject FindOrCreateChild(GameObject parent, string name)
        {
            Transform existing = parent.transform.Find(name);
            if (existing != null) return existing.gameObject;

            var go = new GameObject(name);
            Undo.RegisterCreatedObjectUndo(go, $"Create {name}");
            go.transform.SetParent(parent.transform, false);
            return go;
        }

        private static RectTransform GetOrAddRect(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            if (rt == null) rt = go.AddComponent<RectTransform>();
            return rt;
        }

        private static void SetupImage(GameObject go, UnityEngine.Color color, bool raycast = false)
        {
            GetOrAddRect(go);
            var img = go.GetComponent<Image>();
            if (img == null) img = go.AddComponent<Image>();
            img.color = color;
            img.raycastTarget = raycast;
        }

        private static void SetupImage(GameObject go, UnityEngine.Color color, float alpha)
        {
            color.a = alpha;
            SetupImage(go, color);
        }

        private static void StretchFull(GameObject go)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        private static void StretchWithPadding(GameObject go, float pad)
        {
            var rt = go.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = new Vector2(pad, pad);
            rt.offsetMax = new Vector2(-pad, -pad);
        }

        private static void SetAnchorCenter(RectTransform rt)
        {
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.pivot = new Vector2(0.5f, 0.5f);
        }

        private static void CreateDot(GameObject parent, string name, float xOffset, UnityEngine.Color color)
        {
            var dot = FindOrCreateChild(parent, name);
            SetupImage(dot, color, 0.7f);
            var rt = dot.GetComponent<RectTransform>();
            SetAnchorCenter(rt);
            rt.sizeDelta = new Vector2(6, 6);
            rt.anchoredPosition = new Vector2(xOffset, 0);
        }

        private static void CreateCornerDot(GameObject parent, string name, Vector2 anchor, Vector2 pos)
        {
            var dot = FindOrCreateChild(parent, name);
            SetupImage(dot, BorderMid, 0.5f);
            var rt = dot.GetComponent<RectTransform>();
            rt.anchorMin = anchor;
            rt.anchorMax = anchor;
            rt.pivot = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(6, 6);
            rt.anchoredPosition = pos;
        }

        private static TextMeshProUGUI FindOrCreateTMP(Transform parent, string name, string undoName)
        {
            Transform existing = parent.Find(name);
            GameObject go;
            if (existing != null)
            {
                go = existing.gameObject;
            }
            else
            {
                go = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(go, undoName);
                go.transform.SetParent(parent, false);
            }

            var tmp = go.GetComponent<TextMeshProUGUI>();
            if (tmp == null) tmp = go.AddComponent<TextMeshProUGUI>();
            return tmp;
        }

        private static UnityEngine.Color HexColor(string hex)
        {
            ColorUtility.TryParseHtmlString(hex, out UnityEngine.Color c);
            return c;
        }
    }
}
