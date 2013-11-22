var math = mathjs();
var plotWidth = 10.0;
var createAxis = function (src, dst, colorHex, dashed) {
    var geom = new THREE.Geometry(),
        mat;

    if (dashed) {
        mat = new THREE.LineDashedMaterial(
            {
                linewidth: 1,
                color: colorHex,
                dashSize: 1,
                gapSize: 1
            });
    }
    else {
        mat = new THREE.LineBasicMaterial(
            {
                linewidth: 1.0,
                color: colorHex
            });
    }

    geom.vertices.push(src.clone());
    geom.vertices.push(dst.clone());
    // This one is SUPER important, otherwise 
    // dashed lines will appear as simple plain 
    // lines
    geom.computeLineDistances();

    var axis = new THREE.Line(
        geom, mat, THREE.LinePieces);

    return axis;
}
var createAxes = function (length) {
    var axes = new THREE.Object3D();

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(length, 0, 0),
        'red', false)); // +X

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(-length, 0, 0),
        'red', true)); // -X

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(0, length, 0),
        'blue', false)); // +Y

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(0, -length, 0),
        'blue', true)); // -Y

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(0, 0, length),
        'green', false)); // +Z

    axes.add(createAxis(
        new THREE.Vector3(0, 0, 0),
        new THREE.Vector3(0, 0, -length),
        'green', true)); // -Z

    return axes;
}
var createBox = function () {
    var axes = new THREE.Object3D();

    var height = 5;
    var width = plotWidth / 2.0;

    axes.add(createAxis(
       new THREE.Vector3(width, width, 0.0),
       new THREE.Vector3(width, -width, 0.0),
       'black', false));

    axes.add(createAxis(
    new THREE.Vector3(-width, width, 0.0),
    new THREE.Vector3(-width, -width, 0.0),
    'black', false));


    axes.add(createAxis(
    new THREE.Vector3(width, width, 0.0),
    new THREE.Vector3(-width, width, 0.0),
    'black', false));


    axes.add(createAxis(
    new THREE.Vector3(width, -width, 0.0),
    new THREE.Vector3(-width, -width, 0.0),
    'black', false));


    axes.add(createAxis(
     new THREE.Vector3(width, width, height),
     new THREE.Vector3(width, -width, height),
     'black', false));

    axes.add(createAxis(
    new THREE.Vector3(-width, width, height),
    new THREE.Vector3(-width, -width, height),
    'black', false));


    axes.add(createAxis(
    new THREE.Vector3(width, width, height),
    new THREE.Vector3(-width, width, height),
    'black', false));


    axes.add(createAxis(
    new THREE.Vector3(width, -width, height),
    new THREE.Vector3(-width, -width, height),
    'black', false));


    axes.add(createAxis(
   new THREE.Vector3(width,  width, 0),
   new THREE.Vector3(width, width, height),
   'black', false));

    axes.add(createAxis(
  new THREE.Vector3(-width, width, 0),
  new THREE.Vector3(-width, width, height),
  'black', false));

    axes.add(createAxis(
  new THREE.Vector3(width, -width, 0),
  new THREE.Vector3(width, -width, height),
  'black', false));

    axes.add(createAxis(
  new THREE.Vector3(-width, -width, 0),
  new THREE.Vector3(-width, -width, height),
  'black', false));

    return axes;
}


var scene = new THREE.Scene();
var camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);

controls = new THREE.OrbitControls(camera);
controls.addEventListener('change', render);

var renderer = new THREE.WebGLRenderer({ antialias: true });
renderer.setSize(window.innerWidth, window.innerHeight);
renderer.gammaInput = true;
renderer.gammaOutput = true;
renderer.setClearColor(0xAAAAAA, 1.0);
document.body.appendChild(renderer.domElement);


// Triangle Mesh
var material, geometry, mesh;

material = new THREE.MeshBasicMaterial({ vertexColors: THREE.VertexColors, side: THREE.DoubleSide });
geometry = new THREE.Geometry();

// faces are indexed using characters
var faceIndices = ['a', 'b', 'c', 'd'];

var geometry = new THREE.PlaneGeometry(plotWidth, plotWidth, 100, 100);
for (var i = 0, l = geometry.vertices.length; i < l; i++) {
    var _c = math.complex(geometry.vertices[i].x, geometry.vertices[i].y);
    var z = math.subtract(math.pow(_c, 3), 1); 
    //var z = math.sin(_c);
    var height = math.sqrt(math.add(math.square(math.re(z)), math.square(math.im(z))));

    geometry.vertices[i].z = height/100.0;
    var arg = math.arg(z);
    if (arg < 0) {
        arg = arg + (2.0 * math.pi);
    }
    var h = arg / (2.0 * math.pi);
    // console.log(h);
    var color = new THREE.Color(0xffffff);
    color.setHSL(h, 1.0, 0.5);

    geometry.colors[i] = color;
}

for (var i = 0; i < geometry.faces.length; i++) {
    var face = geometry.faces[i];
    var numberOfSides = (face instanceof THREE.Face3) ? 3 : 4;
    for (var j = 0; j < numberOfSides; j++) {
        var vertexIndex = face[faceIndices[j]];
        face.vertexColors[j] = geometry.colors[vertexIndex];
    }
}

mesh = new THREE.Mesh(geometry, material);

scene.add(mesh);
//scene.add(createAxes(2));
scene.add(createBox());

camera.position.x = 0;
camera.position.y = -10;
camera.position.z = 8;

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