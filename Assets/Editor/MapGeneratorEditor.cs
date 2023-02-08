using UnityEngine;
using UnityEditor;

//Ce sript sert a ajouter les bouton est les fonctionalit� au script MapGenerator, un script editor doit IMPERATIVEMENT ce trouver dans le dossier Editor
[CustomEditor(typeof(MapGenerator))]
public class MapGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        MapGenerator mapGen = (MapGenerator)target;
        if(DrawDefaultInspector())
        {
            //si une valeur a ete chang�
            if(mapGen.autoUpdate)
                mapGen.GenerateMap();
        }
        //on ajoute nos amelio!
        //on ajoute un bouton avec marqu� generate dedans et qui quand on apuis dessus lance la methode generate map
        if(GUILayout.Button("Generate"))
        {
            mapGen.GenerateMap();
        }
    }
}
