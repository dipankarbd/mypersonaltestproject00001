var scene = new THREE.Scene();
var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);

controls = new THREE.OrbitControls(camera);
controls.addEventListener('change', render);

var renderer = new THREE.WebGLRenderer();
renderer.setSize(window.innerWidth, window.innerHeight);
document.body.appendChild(renderer.domElement);

 
// Triangle Mesh
var material, geometry, mesh;

material = new THREE.MeshBasicMaterial({ vertexColors: THREE.VertexColors, side: THREE.DoubleSide }); 
geometry = new THREE.Geometry();
geometry.vertices.push(new THREE.Vector3(0, 0, 0));
geometry.vertices.push(new THREE.Vector3(1, 0, 0));
geometry.vertices.push(new THREE.Vector3(1, 1, 0));
geometry.vertices.push(new THREE.Vector3(0, 1, 0));

geometry.faces.push(new THREE.Face3(0, 1, 2));
geometry.faces.push(new THREE.Face3(0, 2, 3));

var color0 = new THREE.Color(0xFF0000);
var color1 = new THREE.Color(0x00FF00);
var color2 = new THREE.Color(0x0000FF);
var color3 = new THREE.Color(0xFFFF00);

geometry.faces[0].vertexColors = [color0, color1, color2];
geometry.faces[1].vertexColors = [color0, color2, color3];

mesh = new THREE.Mesh(geometry, material);

scene.add(mesh);

camera.position.z = 2;


function onWindowResize() { 
    camera.aspect = window.innerWidth / window.innerHeight;
    camera.updateProjectionMatrix(); 
    renderer.setSize(window.innerWidth, window.innerHeight); 
    render(); 
}
function animate() {
    requestAnimationFrame(animate);
    controls.update(); 
}

function render() {
    renderer.render(scene, camera);
}

animate();