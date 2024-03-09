using Rhino;
using Rhino.Commands;
using Rhino.Geometry;
using Rhino.Input;
using Rhino.Input.Custom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using Rhino.DocObjects;


namespace SelectObjectsByLayerName
{
    public class SelectObjectsByLayerNameCommand : Command
    {
        public SelectObjectsByLayerNameCommand()
        {
            Instance = this;
        }

        public static SelectObjectsByLayerNameCommand Instance { get; private set; }

        public override string EnglishName => "SelectObjectsByLayerName";

        protected override Result RunCommand(RhinoDoc doc, RunMode mode)
        {
            string substring = "";
            Result res = RhinoGet.GetString("レイヤー名に含まれる特定の文字列を入力してください", true, ref substring);
            if (res != Result.Success || string.IsNullOrEmpty(substring))
            {
                RhinoApp.WriteLine("検索がキャンセルされました。");
                return Result.Cancel;
            }
            var targetLayerIds = doc.Layers
                             .Where(layer => layer.FullPath.Contains(substring))
                             .Select(layer => layer.Id)
                             .ToList();
            
            if (!targetLayerIds.Any())
            {
                RhinoApp.WriteLine("指定された文字列を含むレイヤーが見つかりませんでした。");
                return Result.Success;
            }

            foreach (var obj in doc.Objects.GetObjectList(ObjectType.AnyObject))
            {
                var objLayer = doc.Layers.FindIndex(obj.Attributes.LayerIndex);
                if (objLayer != null && targetLayerIds.Contains(objLayer.Id))
                {
                    obj.Select(true);
                }
            }
        
            doc.Views.Redraw();
            return Result.Success;
        }
    }
}
