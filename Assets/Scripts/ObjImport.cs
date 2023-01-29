using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using i5.Toolkit.Core.Caching;
using i5.Toolkit.Core.ModelImporters;
using i5.Toolkit.Core.ServiceCore;

public class ObjImport : MonoBehaviour {
    
    
    void Start() {
        //Register the file cache
        FileCacheService objCache = new FileCacheService();
        ServiceManager.RegisterService(objCache);

        //Register the object importer; set its content loader to the cache aware loader
        ObjImporter importer = new ObjImporter();
        importer.ContentLoader = new CacheAwareContentLoader();
        ServiceManager.RegisterService(importer);
    }

    void Update() {

    }
}
