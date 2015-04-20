# list-of-links
Access Unity assets by string keys

In Unity Editor select Assets->Create->List of Links. This will create ScriptableObject asset.
Add some prefabs or sprites to the list in Inspector.
Create a GameObject with LinkManager component. Add the list to Refs in LinkManager.
Now you can access the linked assets with methods of LinkManager static class:

var prefab = LinkManager.Get(PrefabStringKey);

GameObject instance = LinkManager.GetInstance(ProjectileKey, Vector3.zero, Quaternion.identity);

Icon.sprite = LinkManager.Get(WeaponIconKey) as Sprite;
