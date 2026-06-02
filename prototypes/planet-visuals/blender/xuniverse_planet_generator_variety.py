import bpy
import math
import random

# ==================================================
# XUNIVERSE / VOIDEMPIRES - PLANET PROTOTYPE GENERATOR
# ==================================================
# Cambia este valor para generar un planeta distinto.
# Ejecuta con Alt + P en Blender.
#
# Este script es un prototipo visual local. No está conectado al backend.
# La idea es validar variedad visual antes de llevarlo a PlanetVisualState/Three.js.
# ==================================================

PLANET_PRESET = "continental_earthlike_temperate"

# ==================================================
# OPCIONES DISPONIBLES
# ==================================================
# Lava / volcánicos:
#   "lava_controlado"
#   "lava_obsidiana"
#   "lava_sulfurico"
#
# Gaseosos:
#   "gas_azul"
#   "gas_crema"
#   "gas_verde_toxico"
#   "gas_violeta"
#   "gas_rojo_tormenta"
#   "gas_turquesa"
#   "gas_dorado"
#   "gas_gris_tormentoso"
#
# Continentales / habitables / semi-habitables:
#   "continental_earthlike_temperate"
#   "continental_archipelago"
#   "continental_pangea_dry"
#   "continental_frozen_ocean"
#   "continental_jungle_world"
#   "continental_red_vegetation"
#   "continental_tundra"
#   "continental_swamp"
#
# Rocosos / áridos / cercanos al sol:
#   "mercurio_gris"
#   "mercurio_tostado"
#   "mercurio_agrietado"
#   "rocoso_carbon"
#   "rocoso_cobre"
#   "rocoso_desertico"
#   "rocoso_crateres_blancos"
#   "rocoso_mineral_azul"
#
# Anillos:
#   "anillos_frio"
#   "anillos_rocoso"
#   "anillos_morado"
#   "anillos_gas_azul"
#   "anillos_gas_dorado"
#   "anillos_desertico"
#   "anillos_helado"
#   "anillos_toxico"
#   "anillos_oceanico"
#
# ==================================================

# ==================================================
# LIMPIAR ESCENA
# ==================================================
bpy.ops.object.select_all(action='SELECT')
bpy.ops.object.delete()

scene = bpy.context.scene
scene.render.engine = 'BLENDER_EEVEE'
scene.render.resolution_x = 1400
scene.render.resolution_y = 1400

if hasattr(scene, "eevee"):
    scene.eevee.use_bloom = True
    scene.eevee.bloom_intensity = 0.055
    scene.eevee.bloom_radius = 4

world = scene.world
world.color = (0.002, 0.002, 0.006)

# ==================================================
# UTILIDADES
# ==================================================
def clear_nodes(mat):
    mat.use_nodes = True
    nodes = mat.node_tree.nodes
    links = mat.node_tree.links
    while nodes:
        nodes.remove(nodes[0])
    return nodes, links


def safe_set_input(node, input_name, value):
    if input_name in node.inputs:
        node.inputs[input_name].default_value = value


def create_planet(name, mat, radius=2.0):
    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=128,
        ring_count=64,
        radius=radius,
        location=(0, 0, 0)
    )
    planet = bpy.context.object
    planet.name = name
    planet.data.materials.append(mat)
    bpy.ops.object.shade_smooth()
    return planet


def add_displacement(obj, name, kind='VORONOI', strength=0.012, scale=1.2):
    tex = bpy.data.textures.new(name, type=kind)
    tex.noise_scale = scale

    if hasattr(tex, "intensity"):
        tex.intensity = 0.24

    mod = obj.modifiers.new(name, "DISPLACE")
    mod.strength = strength
    mod.texture = tex


# ==================================================
# MATERIALES BASE
# ==================================================
def create_principled_material(
    name,
    color_a,
    color_b,
    color_c=None,
    noise_scale=5.0,
    noise_detail=10.0,
    roughness=0.75,
    specular=0.25,
    bump_strength=0.045
):
    mat = bpy.data.materials.new(name)
    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (850, 0)

    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.location = (600, 0)
    safe_set_input(bsdf, "Roughness", roughness)
    safe_set_input(bsdf, "Specular", specular)

    texcoord = nodes.new("ShaderNodeTexCoord")
    texcoord.location = (-1050, 0)

    mapping = nodes.new("ShaderNodeMapping")
    mapping.location = (-850, 0)

    noise = nodes.new("ShaderNodeTexNoise")
    noise.location = (-620, 120)
    noise.inputs["Scale"].default_value = noise_scale
    noise.inputs["Detail"].default_value = noise_detail
    noise.inputs["Roughness"].default_value = 0.58

    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.location = (-360, 120)
    ramp.color_ramp.elements[0].position = 0.18
    ramp.color_ramp.elements[0].color = color_a
    ramp.color_ramp.elements[1].position = 0.90
    ramp.color_ramp.elements[1].color = color_b

    if color_c is not None:
        mid = ramp.color_ramp.elements.new(0.52)
        mid.color = color_c

    bump = nodes.new("ShaderNodeBump")
    bump.location = (330, -150)
    bump.inputs["Strength"].default_value = bump_strength
    bump.inputs["Distance"].default_value = 0.065

    links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], noise.inputs["Vector"])
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(noise.outputs["Fac"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])

    return mat


def create_lava_material(name, palette="controlled"):
    mat = bpy.data.materials.new(name)
    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (950, 0)

    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.location = (610, 80)
    safe_set_input(bsdf, "Roughness", 0.88)
    safe_set_input(bsdf, "Specular", 0.18)

    texcoord = nodes.new("ShaderNodeTexCoord")
    texcoord.location = (-1100, 0)

    mapping = nodes.new("ShaderNodeMapping")
    mapping.location = (-900, 0)

    rock_noise = nodes.new("ShaderNodeTexNoise")
    rock_noise.location = (-680, 190)
    rock_noise.inputs["Scale"].default_value = 7.2
    rock_noise.inputs["Detail"].default_value = 12
    rock_noise.inputs["Roughness"].default_value = 0.62

    rock_ramp = nodes.new("ShaderNodeValToRGB")
    rock_ramp.location = (-420, 190)

    lava_ramp = nodes.new("ShaderNodeValToRGB")
    lava_ramp.location = (-420, -110)

    if palette == "obsidian":
        rock_ramp.color_ramp.elements[0].color = (0.004, 0.004, 0.006, 1)
        rock_ramp.color_ramp.elements[1].color = (0.09, 0.02, 0.015, 1)
        lava_ramp.color_ramp.elements[1].color = (0.8, 0.08, 0.02, 1)
        emission_strength = 0.30
    elif palette == "sulfur":
        rock_ramp.color_ramp.elements[0].color = (0.035, 0.030, 0.018, 1)
        rock_ramp.color_ramp.elements[1].color = (0.34, 0.21, 0.04, 1)
        lava_ramp.color_ramp.elements[1].color = (1.0, 0.38, 0.06, 1)
        emission_strength = 0.34
    else:
        rock_ramp.color_ramp.elements[0].color = (0.025, 0.020, 0.018, 1)
        rock_ramp.color_ramp.elements[1].color = (0.26, 0.075, 0.035, 1)
        lava_ramp.color_ramp.elements[1].color = (0.95, 0.18, 0.035, 1)
        emission_strength = 0.38

    rock_ramp.color_ramp.elements[0].position = 0.15
    rock_ramp.color_ramp.elements[1].position = 0.90

    lava_voronoi = nodes.new("ShaderNodeTexVoronoi")
    lava_voronoi.location = (-680, -110)
    lava_voronoi.inputs["Scale"].default_value = 21.0

    lava_ramp.color_ramp.elements[0].position = 0.045
    lava_ramp.color_ramp.elements[0].color = (0.0, 0.0, 0.0, 1)
    lava_ramp.color_ramp.elements[1].position = 0.105
    hot = lava_ramp.color_ramp.elements.new(0.155)
    hot.color = (1.0, 0.48, 0.10, 1)

    mix = nodes.new("ShaderNodeMixRGB")
    mix.location = (-80, 80)
    mix.inputs["Fac"].default_value = 0.24

    emission = nodes.new("ShaderNodeEmission")
    emission.location = (300, -110)
    emission.inputs["Strength"].default_value = emission_strength

    add_shader = nodes.new("ShaderNodeAddShader")
    add_shader.location = (750, 0)

    bump = nodes.new("ShaderNodeBump")
    bump.location = (310, 170)
    bump.inputs["Strength"].default_value = 0.12
    bump.inputs["Distance"].default_value = 0.08

    links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], rock_noise.inputs["Vector"])
    links.new(mapping.outputs["Vector"], lava_voronoi.inputs["Vector"])
    links.new(rock_noise.outputs["Fac"], rock_ramp.inputs["Fac"])
    links.new(lava_voronoi.outputs["Distance"], lava_ramp.inputs["Fac"])
    links.new(rock_ramp.outputs["Color"], mix.inputs["Color1"])
    links.new(lava_ramp.outputs["Color"], mix.inputs["Color2"])
    links.new(rock_noise.outputs["Fac"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(lava_ramp.outputs["Color"], emission.inputs["Color"])
    links.new(bsdf.outputs["BSDF"], add_shader.inputs[0])
    links.new(emission.outputs["Emission"], add_shader.inputs[1])
    links.new(add_shader.outputs["Shader"], output.inputs["Surface"])

    return mat


def create_gas_material(name, palette):
    mat = bpy.data.materials.new(name)
    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (900, 0)

    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.location = (640, 0)
    safe_set_input(bsdf, "Roughness", 0.68)
    safe_set_input(bsdf, "Specular", 0.18)

    texcoord = nodes.new("ShaderNodeTexCoord")
    texcoord.location = (-1100, 0)

    mapping = nodes.new("ShaderNodeMapping")
    mapping.location = (-900, 0)
    mapping.inputs["Scale"].default_value[0] = 0.75
    mapping.inputs["Scale"].default_value[1] = 4.5
    mapping.inputs["Scale"].default_value[2] = 1.0

    bands = nodes.new("ShaderNodeTexWave")
    bands.location = (-650, 150)
    bands.inputs["Scale"].default_value = palette.get("wave_scale", 8.0)
    bands.inputs["Distortion"].default_value = palette.get("distortion", 12.0)

    noise = nodes.new("ShaderNodeTexNoise")
    noise.location = (-650, -120)
    noise.inputs["Scale"].default_value = palette.get("noise_scale", 5.0)
    noise.inputs["Detail"].default_value = 12.0
    noise.inputs["Roughness"].default_value = 0.55

    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.location = (-360, 80)

    colors = palette["colors"]
    ramp.color_ramp.elements[0].position = colors[0][0]
    ramp.color_ramp.elements[0].color = colors[0][1]
    ramp.color_ramp.elements[1].position = colors[-1][0]
    ramp.color_ramp.elements[1].color = colors[-1][1]

    for pos, color in colors[1:-1]:
        e = ramp.color_ramp.elements.new(pos)
        e.color = color

    bump = nodes.new("ShaderNodeBump")
    bump.location = (350, -150)
    bump.inputs["Strength"].default_value = 0.035
    bump.inputs["Distance"].default_value = 0.04

    links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], bands.inputs["Vector"])
    links.new(mapping.outputs["Vector"], noise.inputs["Vector"])
    links.new(bands.outputs["Color"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(noise.outputs["Fac"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])

    return mat


def create_continental_material(name, profile):
    """
    Material procedimental tipo planeta continental.
    Mezcla océano/tierra con una máscara de ruido y colores por bioma.
    No pretende ser geografía real; sirve como prototipo visual.
    """
    mat = bpy.data.materials.new(name)
    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (1000, 0)

    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.location = (740, 0)
    safe_set_input(bsdf, "Roughness", profile.get("roughness", 0.72))
    safe_set_input(bsdf, "Specular", profile.get("specular", 0.25))

    texcoord = nodes.new("ShaderNodeTexCoord")
    texcoord.location = (-1200, 0)

    mapping = nodes.new("ShaderNodeMapping")
    mapping.location = (-1000, 0)

    land_noise = nodes.new("ShaderNodeTexNoise")
    land_noise.location = (-780, 220)
    land_noise.inputs["Scale"].default_value = profile.get("land_scale", 2.7)
    land_noise.inputs["Detail"].default_value = 12.0
    land_noise.inputs["Roughness"].default_value = 0.58

    land_mask = nodes.new("ShaderNodeValToRGB")
    land_mask.location = (-520, 220)
    land_mask.color_ramp.elements[0].position = profile.get("sea_ratio_low", 0.43)
    land_mask.color_ramp.elements[0].color = (0, 0, 0, 1)
    land_mask.color_ramp.elements[1].position = profile.get("sea_ratio_high", 0.56)
    land_mask.color_ramp.elements[1].color = (1, 1, 1, 1)

    biome_noise = nodes.new("ShaderNodeTexNoise")
    biome_noise.location = (-780, -60)
    biome_noise.inputs["Scale"].default_value = profile.get("biome_scale", 8.5)
    biome_noise.inputs["Detail"].default_value = 13.0
    biome_noise.inputs["Roughness"].default_value = 0.62

    land_ramp = nodes.new("ShaderNodeValToRGB")
    land_ramp.location = (-520, -60)
    land_colors = profile["land_colors"]
    land_ramp.color_ramp.elements[0].position = land_colors[0][0]
    land_ramp.color_ramp.elements[0].color = land_colors[0][1]
    land_ramp.color_ramp.elements[1].position = land_colors[-1][0]
    land_ramp.color_ramp.elements[1].color = land_colors[-1][1]

    for pos, color in land_colors[1:-1]:
        e = land_ramp.color_ramp.elements.new(pos)
        e.color = color

    ocean_noise = nodes.new("ShaderNodeTexNoise")
    ocean_noise.location = (-780, -340)
    ocean_noise.inputs["Scale"].default_value = profile.get("ocean_scale", 10.0)
    ocean_noise.inputs["Detail"].default_value = 8.0
    ocean_noise.inputs["Roughness"].default_value = 0.48

    ocean_ramp = nodes.new("ShaderNodeValToRGB")
    ocean_ramp.location = (-520, -340)
    ocean_colors = profile["ocean_colors"]
    ocean_ramp.color_ramp.elements[0].position = ocean_colors[0][0]
    ocean_ramp.color_ramp.elements[0].color = ocean_colors[0][1]
    ocean_ramp.color_ramp.elements[1].position = ocean_colors[-1][0]
    ocean_ramp.color_ramp.elements[1].color = ocean_colors[-1][1]

    mix = nodes.new("ShaderNodeMixRGB")
    mix.location = (110, 60)
    mix.blend_type = 'MIX'

    bump = nodes.new("ShaderNodeBump")
    bump.location = (420, -180)
    bump.inputs["Strength"].default_value = profile.get("bump_strength", 0.055)
    bump.inputs["Distance"].default_value = 0.06

    links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], land_noise.inputs["Vector"])
    links.new(mapping.outputs["Vector"], biome_noise.inputs["Vector"])
    links.new(mapping.outputs["Vector"], ocean_noise.inputs["Vector"])

    links.new(land_noise.outputs["Fac"], land_mask.inputs["Fac"])
    links.new(biome_noise.outputs["Fac"], land_ramp.inputs["Fac"])
    links.new(ocean_noise.outputs["Fac"], ocean_ramp.inputs["Fac"])

    links.new(land_mask.outputs["Color"], mix.inputs["Fac"])
    links.new(ocean_ramp.outputs["Color"], mix.inputs["Color1"])
    links.new(land_ramp.outputs["Color"], mix.inputs["Color2"])

    links.new(land_noise.outputs["Fac"], bump.inputs["Height"])
    links.new(bump.outputs["Normal"], bsdf.inputs["Normal"])
    links.new(mix.outputs["Color"], bsdf.inputs["Base Color"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])

    return mat


def create_cloud_material(name, alpha=0.38, scale=7.0):
    mat = bpy.data.materials.new(name)
    mat.blend_method = 'BLEND'
    mat.show_transparent_back = False

    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (700, 0)

    bsdf = nodes.new("ShaderNodeBsdfPrincipled")
    bsdf.location = (450, 0)
    bsdf.inputs["Base Color"].default_value = (1, 1, 1, 1)
    safe_set_input(bsdf, "Roughness", 1.0)
    if "Alpha" in bsdf.inputs:
        bsdf.inputs["Alpha"].default_value = alpha

    texcoord = nodes.new("ShaderNodeTexCoord")
    texcoord.location = (-750, 0)

    mapping = nodes.new("ShaderNodeMapping")
    mapping.location = (-550, 0)

    noise = nodes.new("ShaderNodeTexNoise")
    noise.location = (-350, 0)
    noise.inputs["Scale"].default_value = scale
    noise.inputs["Detail"].default_value = 11.0
    noise.inputs["Roughness"].default_value = 0.55

    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.location = (-100, 0)
    ramp.color_ramp.elements[0].position = 0.50
    ramp.color_ramp.elements[0].color = (0, 0, 0, 0)
    ramp.color_ramp.elements[1].position = 0.68
    ramp.color_ramp.elements[1].color = (1, 1, 1, alpha)

    links.new(texcoord.outputs["Generated"], mapping.inputs["Vector"])
    links.new(mapping.outputs["Vector"], noise.inputs["Vector"])
    links.new(noise.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Alpha"], bsdf.inputs["Alpha"])
    links.new(bsdf.outputs["BSDF"], output.inputs["Surface"])

    return mat


def add_cloud_layer(radius=2.035, alpha=0.35, scale=7.0):
    mat = create_cloud_material("Cloud_Layer_Material", alpha=alpha, scale=scale)

    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=128,
        ring_count=64,
        radius=radius,
        location=(0, 0, 0)
    )

    clouds = bpy.context.object
    clouds.name = "Cloud_Layer"
    clouds.data.materials.append(mat)
    bpy.ops.object.shade_smooth()
    clouds.rotation_euler = (math.radians(-8), 0, math.radians(35))
    return clouds


# ==================================================
# ATMÓSFERA, ANILLOS, FONDO Y LUCES
# ==================================================
def create_atmosphere_material(name, color, strength=0.16):
    mat = bpy.data.materials.new(name)
    mat.blend_method = 'BLEND'
    mat.show_transparent_back = True

    nodes, links = clear_nodes(mat)

    output = nodes.new("ShaderNodeOutputMaterial")
    output.location = (650, 0)

    transparent = nodes.new("ShaderNodeBsdfTransparent")
    transparent.location = (220, 100)

    emission = nodes.new("ShaderNodeEmission")
    emission.location = (220, -100)
    emission.inputs["Color"].default_value = color
    emission.inputs["Strength"].default_value = strength

    fresnel = nodes.new("ShaderNodeFresnel")
    fresnel.location = (-280, 0)
    fresnel.inputs["IOR"].default_value = 1.15

    ramp = nodes.new("ShaderNodeValToRGB")
    ramp.location = (-40, 0)
    ramp.color_ramp.elements[0].position = 0.30
    ramp.color_ramp.elements[0].color = (0, 0, 0, 1)
    ramp.color_ramp.elements[1].position = 0.95
    ramp.color_ramp.elements[1].color = (1, 1, 1, 1)

    mix = nodes.new("ShaderNodeMixShader")
    mix.location = (460, 0)

    links.new(fresnel.outputs["Fac"], ramp.inputs["Fac"])
    links.new(ramp.outputs["Color"], mix.inputs["Fac"])
    links.new(transparent.outputs["BSDF"], mix.inputs[1])
    links.new(emission.outputs["Emission"], mix.inputs[2])
    links.new(mix.outputs["Shader"], output.inputs["Surface"])

    return mat


def add_atmosphere(radius, color, strength=0.15):
    atm_mat = create_atmosphere_material("Atmosphere_Material", color, strength)

    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=128,
        ring_count=64,
        radius=radius,
        location=(0, 0, 0)
    )

    atm = bpy.context.object
    atm.name = "Soft_Atmosphere"
    atm.data.materials.append(atm_mat)
    bpy.ops.object.shade_smooth()
    return atm


def create_ring_material(name, color, alpha=0.42):
    mat = bpy.data.materials.new(name)
    mat.diffuse_color = color
    mat.blend_method = 'BLEND'
    mat.show_transparent_back = True

    mat.use_nodes = True
    bsdf = mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = color
        if "Alpha" in bsdf.inputs:
            bsdf.inputs["Alpha"].default_value = alpha
        safe_set_input(bsdf, "Roughness", 0.8)
        safe_set_input(bsdf, "Specular", 0.15)

    return mat


def add_rings(profile):
    ring_color = profile["ring_color"]
    ring_mat = create_ring_material("Ring_Material", ring_color, profile.get("ring_alpha", 0.42))

    count = profile.get("ring_count", 5)
    base_major = profile.get("ring_major", 3.05)
    random.seed(profile.get("stars_seed", 10) + 100)

    for i in range(count):
        major = base_major + i * profile.get("ring_gap", 0.18)
        minor = profile.get("ring_minor", 0.028) * random.uniform(0.7, 1.25)

        bpy.ops.mesh.primitive_torus_add(
            major_radius=major,
            minor_radius=minor,
            major_segments=192,
            minor_segments=8,
            location=(0, 0, 0)
        )

        ring = bpy.context.object
        ring.name = f"Planet_Ring_{i+1}"
        ring.data.materials.append(ring_mat)
        ring.rotation_euler = (
            math.radians(profile.get("ring_tilt_x", 68)),
            math.radians(profile.get("ring_tilt_y", 0)),
            math.radians(profile.get("ring_tilt_z", 18))
        )


def add_starfield(seed=10, count=100):
    mat = bpy.data.materials.new("Background_Stars")
    mat.diffuse_color = (1.0, 1.0, 1.0, 1)

    random.seed(seed)

    for i in range(count):
        angle = random.uniform(0, math.tau)
        r = random.uniform(8, 13)
        z = random.uniform(-4.5, 4.5)
        x = math.cos(angle) * r
        y = math.sin(angle) * r

        bpy.ops.mesh.primitive_uv_sphere_add(
            segments=8,
            ring_count=4,
            radius=random.uniform(0.008, 0.022),
            location=(x, y, z)
        )

        star = bpy.context.object
        star.name = "Background_Star"
        star.data.materials.append(mat)


def add_near_sun_hint():
    sun_mat = bpy.data.materials.new("Distant_Sun_Disc_Material")
    sun_mat.diffuse_color = (1.0, 0.72, 0.32, 1)

    sun_mat.use_nodes = True
    bsdf = sun_mat.node_tree.nodes.get("Principled BSDF")
    if bsdf:
        bsdf.inputs["Base Color"].default_value = (1.0, 0.72, 0.32, 1)
        safe_set_input(bsdf, "Roughness", 0.35)

    bpy.ops.mesh.primitive_uv_sphere_add(
        segments=64,
        ring_count=32,
        radius=0.65,
        location=(-4.8, -4.8, 2.2)
    )

    sun_disc = bpy.context.object
    sun_disc.name = "Distant_Sun_Visual_Hint"
    sun_disc.data.materials.append(sun_mat)
    bpy.ops.object.shade_smooth()


def setup_lighting(near_sun=False):
    if near_sun:
        bpy.ops.object.light_add(type='SUN', location=(-6, -7, 5))
        sun = bpy.context.object
        sun.name = "Near_Sun_Strong_Light"
        sun.data.energy = 4.4
        sun.rotation_euler = (math.radians(52), 0, math.radians(-35))

        bpy.ops.object.light_add(type='AREA', location=(3, -5, 2))
        fill = bpy.context.object
        fill.name = "Weak_Fill_Light"
        fill.data.energy = 20
        fill.data.size = 5
    else:
        bpy.ops.object.light_add(type='SUN', location=(5, -6, 5))
        sun = bpy.context.object
        sun.name = "Main_Sun"
        sun.data.energy = 3.0
        sun.rotation_euler = (math.radians(45), 0, math.radians(35))

        bpy.ops.object.light_add(type='AREA', location=(-4, -5, 3))
        fill = bpy.context.object
        fill.name = "Soft_Fill"
        fill.data.energy = 65
        fill.data.size = 6


def setup_camera():
    bpy.ops.object.camera_add(
        location=(0, -6.4, 1.35),
        rotation=(math.radians(78), 0, 0)
    )

    camera = bpy.context.object
    camera.name = "Camera"
    camera.data.lens = 55
    scene.camera = camera


# ==================================================
# PALETAS Y PRESETS
# ==================================================
GAS_PALETTES = {
    "blue": {
        "colors": [
            (0.08, (0.03, 0.05, 0.18, 1)),
            (0.35, (0.10, 0.35, 0.72, 1)),
            (0.58, (0.48, 0.78, 1.0, 1)),
            (0.78, (0.12, 0.18, 0.44, 1)),
            (0.92, (0.02, 0.06, 0.20, 1))
        ],
        "wave_scale": 8.0,
        "distortion": 12.0
    },
    "cream": {
        "colors": [
            (0.08, (0.30, 0.17, 0.08, 1)),
            (0.28, (0.75, 0.42, 0.18, 1)),
            (0.52, (0.95, 0.78, 0.48, 1)),
            (0.72, (0.55, 0.30, 0.13, 1)),
            (0.92, (0.18, 0.10, 0.06, 1))
        ],
        "wave_scale": 7.2,
        "distortion": 10.0
    },
    "green_toxic": {
        "colors": [
            (0.08, (0.02, 0.12, 0.05, 1)),
            (0.32, (0.20, 0.55, 0.14, 1)),
            (0.55, (0.70, 0.95, 0.28, 1)),
            (0.78, (0.08, 0.28, 0.10, 1)),
            (0.92, (0.01, 0.07, 0.03, 1))
        ],
        "wave_scale": 9.5,
        "distortion": 14.0
    },
    "violet": {
        "colors": [
            (0.08, (0.05, 0.01, 0.12, 1)),
            (0.34, (0.26, 0.07, 0.48, 1)),
            (0.58, (0.76, 0.30, 1.0, 1)),
            (0.76, (0.18, 0.05, 0.36, 1)),
            (0.92, (0.04, 0.01, 0.09, 1))
        ],
        "wave_scale": 8.6,
        "distortion": 16.0
    },
    "red_storm": {
        "colors": [
            (0.08, (0.16, 0.03, 0.02, 1)),
            (0.32, (0.58, 0.12, 0.05, 1)),
            (0.56, (0.95, 0.50, 0.18, 1)),
            (0.72, (0.35, 0.05, 0.03, 1)),
            (0.92, (0.10, 0.02, 0.015, 1))
        ],
        "wave_scale": 7.8,
        "distortion": 18.0
    },
    "turquoise": {
        "colors": [
            (0.08, (0.02, 0.10, 0.12, 1)),
            (0.32, (0.05, 0.42, 0.46, 1)),
            (0.56, (0.38, 0.95, 0.95, 1)),
            (0.74, (0.04, 0.24, 0.38, 1)),
            (0.92, (0.01, 0.05, 0.10, 1))
        ],
        "wave_scale": 8.1,
        "distortion": 12.0
    },
    "gold": {
        "colors": [
            (0.08, (0.20, 0.11, 0.02, 1)),
            (0.30, (0.76, 0.48, 0.10, 1)),
            (0.54, (1.0, 0.86, 0.38, 1)),
            (0.74, (0.44, 0.24, 0.05, 1)),
            (0.92, (0.12, 0.07, 0.02, 1))
        ],
        "wave_scale": 6.8,
        "distortion": 9.0
    },
    "grey_storm": {
        "colors": [
            (0.08, (0.06, 0.07, 0.08, 1)),
            (0.30, (0.28, 0.31, 0.35, 1)),
            (0.55, (0.78, 0.82, 0.86, 1)),
            (0.76, (0.18, 0.20, 0.24, 1)),
            (0.92, (0.04, 0.045, 0.05, 1))
        ],
        "wave_scale": 9.0,
        "distortion": 20.0
    }
}

CONTINENTAL_PROFILES = {
    "temperate": {
        "land_colors": [
            (0.12, (0.07, 0.24, 0.07, 1)),
            (0.46, (0.18, 0.46, 0.13, 1)),
            (0.64, (0.44, 0.36, 0.16, 1)),
            (0.90, (0.68, 0.64, 0.50, 1))
        ],
        "ocean_colors": [(0.10, (0.005, 0.04, 0.22, 1)), (0.95, (0.02, 0.28, 0.68, 1))],
        "land_scale": 2.8,
        "sea_ratio_low": 0.43,
        "sea_ratio_high": 0.56,
        "clouds": True,
        "cloud_alpha": 0.36,
        "atmosphere": (0.12, 0.50, 1.0, 1),
        "atmosphere_strength": 0.16
    },
    "archipelago": {
        "land_colors": [
            (0.12, (0.10, 0.32, 0.09, 1)),
            (0.55, (0.35, 0.55, 0.16, 1)),
            (0.90, (0.70, 0.62, 0.36, 1))
        ],
        "ocean_colors": [(0.10, (0.0, 0.08, 0.26, 1)), (0.95, (0.0, 0.58, 0.82, 1))],
        "land_scale": 5.6,
        "sea_ratio_low": 0.55,
        "sea_ratio_high": 0.66,
        "clouds": True,
        "cloud_alpha": 0.30,
        "atmosphere": (0.10, 0.58, 1.0, 1),
        "atmosphere_strength": 0.17
    },
    "pangea_dry": {
        "land_colors": [
            (0.12, (0.32, 0.20, 0.09, 1)),
            (0.46, (0.66, 0.48, 0.20, 1)),
            (0.78, (0.82, 0.68, 0.34, 1)),
            (0.92, (0.38, 0.26, 0.12, 1))
        ],
        "ocean_colors": [(0.10, (0.0, 0.04, 0.16, 1)), (0.95, (0.0, 0.17, 0.42, 1))],
        "land_scale": 1.7,
        "sea_ratio_low": 0.35,
        "sea_ratio_high": 0.46,
        "clouds": True,
        "cloud_alpha": 0.20,
        "atmosphere": (0.75, 0.52, 0.26, 1),
        "atmosphere_strength": 0.08
    },
    "frozen_ocean": {
        "land_colors": [
            (0.12, (0.48, 0.62, 0.72, 1)),
            (0.52, (0.82, 0.92, 1.0, 1)),
            (0.88, (0.38, 0.50, 0.60, 1))
        ],
        "ocean_colors": [(0.10, (0.02, 0.07, 0.18, 1)), (0.95, (0.24, 0.48, 0.70, 1))],
        "land_scale": 3.4,
        "sea_ratio_low": 0.48,
        "sea_ratio_high": 0.60,
        "clouds": True,
        "cloud_alpha": 0.28,
        "atmosphere": (0.42, 0.72, 1.0, 1),
        "atmosphere_strength": 0.14
    },
    "jungle": {
        "land_colors": [
            (0.12, (0.02, 0.16, 0.04, 1)),
            (0.44, (0.05, 0.36, 0.07, 1)),
            (0.72, (0.20, 0.55, 0.10, 1)),
            (0.92, (0.08, 0.24, 0.06, 1))
        ],
        "ocean_colors": [(0.10, (0.0, 0.07, 0.18, 1)), (0.95, (0.0, 0.34, 0.42, 1))],
        "land_scale": 3.0,
        "sea_ratio_low": 0.40,
        "sea_ratio_high": 0.53,
        "clouds": True,
        "cloud_alpha": 0.44,
        "atmosphere": (0.10, 0.60, 0.42, 1),
        "atmosphere_strength": 0.14
    },
    "red_vegetation": {
        "land_colors": [
            (0.12, (0.18, 0.02, 0.04, 1)),
            (0.50, (0.55, 0.08, 0.16, 1)),
            (0.76, (0.78, 0.28, 0.30, 1)),
            (0.92, (0.30, 0.06, 0.09, 1))
        ],
        "ocean_colors": [(0.10, (0.02, 0.02, 0.14, 1)), (0.95, (0.12, 0.10, 0.34, 1))],
        "land_scale": 2.9,
        "sea_ratio_low": 0.42,
        "sea_ratio_high": 0.55,
        "clouds": True,
        "cloud_alpha": 0.26,
        "atmosphere": (0.65, 0.22, 0.45, 1),
        "atmosphere_strength": 0.11
    },
    "tundra": {
        "land_colors": [
            (0.12, (0.30, 0.38, 0.30, 1)),
            (0.44, (0.52, 0.58, 0.50, 1)),
            (0.70, (0.82, 0.86, 0.80, 1)),
            (0.92, (0.24, 0.30, 0.25, 1))
        ],
        "ocean_colors": [(0.10, (0.0, 0.05, 0.15, 1)), (0.95, (0.10, 0.30, 0.50, 1))],
        "land_scale": 3.3,
        "sea_ratio_low": 0.43,
        "sea_ratio_high": 0.56,
        "clouds": True,
        "cloud_alpha": 0.34,
        "atmosphere": (0.42, 0.66, 0.90, 1),
        "atmosphere_strength": 0.12
    },
    "swamp": {
        "land_colors": [
            (0.12, (0.03, 0.12, 0.05, 1)),
            (0.48, (0.14, 0.26, 0.08, 1)),
            (0.72, (0.25, 0.36, 0.14, 1)),
            (0.92, (0.07, 0.16, 0.06, 1))
        ],
        "ocean_colors": [(0.10, (0.02, 0.08, 0.08, 1)), (0.95, (0.08, 0.26, 0.20, 1))],
        "land_scale": 4.2,
        "sea_ratio_low": 0.38,
        "sea_ratio_high": 0.51,
        "clouds": True,
        "cloud_alpha": 0.46,
        "atmosphere": (0.18, 0.52, 0.32, 1),
        "atmosphere_strength": 0.10
    }
}

PRESETS = {
    # Lava
    "lava_controlado": {"kind": "lava", "name": "Lava_Planet_Controlled", "lava_palette": "controlled", "atmosphere": (0.9, 0.20, 0.06, 1), "atmosphere_strength": 0.08, "displacement_strength": 0.020, "near_sun": False, "stars_seed": 3},
    "lava_obsidiana": {"kind": "lava", "name": "Obsidian_Lava_Planet", "lava_palette": "obsidian", "atmosphere": (0.8, 0.10, 0.04, 1), "atmosphere_strength": 0.06, "displacement_strength": 0.024, "near_sun": False, "stars_seed": 4},
    "lava_sulfurico": {"kind": "lava", "name": "Sulfuric_Lava_Planet", "lava_palette": "sulfur", "atmosphere": (1.0, 0.45, 0.08, 1), "atmosphere_strength": 0.07, "displacement_strength": 0.022, "near_sun": False, "stars_seed": 5},

    # Gas
    "gas_azul": {"kind": "gas", "name": "Blue_Gas_Giant", "gas_palette": "blue", "atmosphere": (0.20, 0.50, 1.0, 1), "atmosphere_strength": 0.15, "near_sun": False, "stars_seed": 12},
    "gas_crema": {"kind": "gas", "name": "Cream_Gas_Giant", "gas_palette": "cream", "atmosphere": (1.0, 0.58, 0.25, 1), "atmosphere_strength": 0.11, "near_sun": False, "stars_seed": 13},
    "gas_verde_toxico": {"kind": "gas", "name": "Toxic_Green_Gas_Giant", "gas_palette": "green_toxic", "atmosphere": (0.35, 1.0, 0.20, 1), "atmosphere_strength": 0.12, "near_sun": False, "stars_seed": 14},
    "gas_violeta": {"kind": "gas", "name": "Violet_Gas_Giant", "gas_palette": "violet", "atmosphere": (0.65, 0.20, 1.0, 1), "atmosphere_strength": 0.13, "near_sun": False, "stars_seed": 15},
    "gas_rojo_tormenta": {"kind": "gas", "name": "Red_Storm_Gas_Giant", "gas_palette": "red_storm", "atmosphere": (1.0, 0.26, 0.10, 1), "atmosphere_strength": 0.10, "near_sun": False, "stars_seed": 16},
    "gas_turquesa": {"kind": "gas", "name": "Turquoise_Gas_Giant", "gas_palette": "turquoise", "atmosphere": (0.10, 0.90, 1.0, 1), "atmosphere_strength": 0.13, "near_sun": False, "stars_seed": 17},
    "gas_dorado": {"kind": "gas", "name": "Golden_Gas_Giant", "gas_palette": "gold", "atmosphere": (1.0, 0.70, 0.20, 1), "atmosphere_strength": 0.10, "near_sun": False, "stars_seed": 18},
    "gas_gris_tormentoso": {"kind": "gas", "name": "Grey_Storm_Gas_Giant", "gas_palette": "grey_storm", "atmosphere": (0.60, 0.70, 0.85, 1), "atmosphere_strength": 0.09, "near_sun": False, "stars_seed": 19},

    # Continentales
    "continental_earthlike_temperate": {"kind": "continental", "name": "Continental_Temperate_World", "continental_profile": "temperate", "displacement_strength": 0.010, "stars_seed": 41},
    "continental_archipelago": {"kind": "continental", "name": "Continental_Archipelago_World", "continental_profile": "archipelago", "displacement_strength": 0.008, "stars_seed": 42},
    "continental_pangea_dry": {"kind": "continental", "name": "Continental_Dry_Pangea_World", "continental_profile": "pangea_dry", "displacement_strength": 0.014, "stars_seed": 43},
    "continental_frozen_ocean": {"kind": "continental", "name": "Continental_Frozen_Ocean_World", "continental_profile": "frozen_ocean", "displacement_strength": 0.012, "stars_seed": 44},
    "continental_jungle_world": {"kind": "continental", "name": "Continental_Jungle_World", "continental_profile": "jungle", "displacement_strength": 0.012, "stars_seed": 45},
    "continental_red_vegetation": {"kind": "continental", "name": "Continental_Red_Vegetation_World", "continental_profile": "red_vegetation", "displacement_strength": 0.010, "stars_seed": 46},
    "continental_tundra": {"kind": "continental", "name": "Continental_Tundra_World", "continental_profile": "tundra", "displacement_strength": 0.014, "stars_seed": 47},
    "continental_swamp": {"kind": "continental", "name": "Continental_Swamp_World", "continental_profile": "swamp", "displacement_strength": 0.010, "stars_seed": 48},

    # Rocosos / Mercurio
    "mercurio_gris": {"kind": "rocky", "name": "Mercury_Style_Grey", "material": {"color_a": (0.09, 0.085, 0.080, 1), "color_b": (0.58, 0.55, 0.50, 1), "color_c": (0.28, 0.27, 0.25, 1), "noise_scale": 11.0, "roughness": 0.92, "specular": 0.10, "bump_strength": 0.10}, "atmosphere": None, "displacement_strength": 0.030, "near_sun": True, "stars_seed": 31},
    "mercurio_tostado": {"kind": "rocky", "name": "Mercury_Style_Scorched", "material": {"color_a": (0.13, 0.075, 0.045, 1), "color_b": (0.62, 0.42, 0.24, 1), "color_c": (0.32, 0.20, 0.12, 1), "noise_scale": 12.5, "roughness": 0.95, "specular": 0.08, "bump_strength": 0.12}, "atmosphere": None, "displacement_strength": 0.034, "near_sun": True, "stars_seed": 32},
    "mercurio_agrietado": {"kind": "rocky", "name": "Mercury_Style_Cracked", "material": {"color_a": (0.045, 0.040, 0.038, 1), "color_b": (0.44, 0.40, 0.35, 1), "color_c": (0.18, 0.16, 0.145, 1), "noise_scale": 15.0, "roughness": 0.96, "specular": 0.05, "bump_strength": 0.15}, "atmosphere": None, "displacement_strength": 0.040, "near_sun": True, "stars_seed": 33},
    "rocoso_carbon": {"kind": "rocky", "name": "Carbon_Rocky_World", "material": {"color_a": (0.015, 0.015, 0.016, 1), "color_b": (0.20, 0.20, 0.22, 1), "color_c": (0.07, 0.07, 0.08, 1), "noise_scale": 13.0, "roughness": 0.94, "specular": 0.06, "bump_strength": 0.14}, "atmosphere": None, "displacement_strength": 0.038, "near_sun": False, "stars_seed": 34},
    "rocoso_cobre": {"kind": "rocky", "name": "Copper_Rocky_World", "material": {"color_a": (0.16, 0.07, 0.03, 1), "color_b": (0.64, 0.28, 0.10, 1), "color_c": (0.34, 0.13, 0.05, 1), "noise_scale": 10.5, "roughness": 0.90, "specular": 0.12, "bump_strength": 0.12}, "atmosphere": (0.70, 0.35, 0.12, 1), "atmosphere_strength": 0.045, "displacement_strength": 0.030, "near_sun": False, "stars_seed": 35},
    "rocoso_desertico": {"kind": "rocky", "name": "Desert_Rocky_World", "material": {"color_a": (0.28, 0.18, 0.08, 1), "color_b": (0.82, 0.58, 0.26, 1), "color_c": (0.52, 0.36, 0.16, 1), "noise_scale": 9.0, "roughness": 0.96, "specular": 0.08, "bump_strength": 0.09}, "atmosphere": (0.90, 0.55, 0.25, 1), "atmosphere_strength": 0.06, "displacement_strength": 0.026, "near_sun": False, "stars_seed": 36},
    "rocoso_crateres_blancos": {"kind": "rocky", "name": "White_Crater_Rocky_World", "material": {"color_a": (0.22, 0.22, 0.22, 1), "color_b": (0.78, 0.76, 0.70, 1), "color_c": (0.46, 0.44, 0.40, 1), "noise_scale": 14.0, "roughness": 0.98, "specular": 0.05, "bump_strength": 0.16}, "atmosphere": None, "displacement_strength": 0.043, "near_sun": False, "stars_seed": 37},
    "rocoso_mineral_azul": {"kind": "rocky", "name": "Blue_Mineral_Rocky_World", "material": {"color_a": (0.04, 0.06, 0.08, 1), "color_b": (0.20, 0.42, 0.58, 1), "color_c": (0.08, 0.16, 0.25, 1), "noise_scale": 12.0, "roughness": 0.88, "specular": 0.16, "bump_strength": 0.11}, "atmosphere": (0.16, 0.46, 0.90, 1), "atmosphere_strength": 0.07, "displacement_strength": 0.030, "near_sun": False, "stars_seed": 38},

    # Anillos existentes y nuevos
    "anillos_frio": {"kind": "ringed", "name": "Cold_Ringed_Planet", "material": {"color_a": (0.08, 0.13, 0.28, 1), "color_b": (0.55, 0.78, 1.0, 1), "color_c": (0.22, 0.42, 0.75, 1), "noise_scale": 6.0, "roughness": 0.60, "specular": 0.35}, "atmosphere": (0.25, 0.55, 1.0, 1), "atmosphere_strength": 0.13, "rings": True, "ring_color": (0.66, 0.82, 1.0, 0.38), "ring_alpha": 0.38, "ring_count": 6, "ring_major": 3.05, "ring_minor": 0.022, "ring_tilt_x": 70, "ring_tilt_z": 22, "near_sun": False, "stars_seed": 21},
    "anillos_rocoso": {"kind": "ringed", "name": "Rocky_Ringed_Planet", "material": {"color_a": (0.12, 0.09, 0.07, 1), "color_b": (0.58, 0.43, 0.30, 1), "color_c": (0.30, 0.23, 0.17, 1), "noise_scale": 8.0, "roughness": 0.82, "specular": 0.18}, "atmosphere": (0.85, 0.55, 0.30, 1), "atmosphere_strength": 0.07, "rings": True, "ring_color": (0.72, 0.58, 0.43, 0.36), "ring_alpha": 0.36, "ring_count": 7, "ring_major": 3.00, "ring_minor": 0.024, "ring_tilt_x": 66, "ring_tilt_z": -18, "near_sun": False, "stars_seed": 22},
    "anillos_morado": {"kind": "ringed", "name": "Purple_Ringed_Planet", "material": {"color_a": (0.08, 0.02, 0.16, 1), "color_b": (0.62, 0.20, 0.88, 1), "color_c": (0.25, 0.08, 0.46, 1), "noise_scale": 5.5, "roughness": 0.68, "specular": 0.30}, "atmosphere": (0.60, 0.22, 1.0, 1), "atmosphere_strength": 0.12, "rings": True, "ring_color": (0.70, 0.42, 1.0, 0.35), "ring_alpha": 0.35, "ring_count": 5, "ring_major": 3.12, "ring_minor": 0.025, "ring_tilt_x": 72, "ring_tilt_z": 14, "near_sun": False, "stars_seed": 23},
    "anillos_gas_azul": {"kind": "gas_ringed", "name": "Blue_Gas_Ringed_World", "gas_palette": "blue", "atmosphere": (0.20, 0.50, 1.0, 1), "atmosphere_strength": 0.13, "rings": True, "ring_color": (0.55, 0.75, 1.0, 0.34), "ring_alpha": 0.34, "ring_count": 8, "ring_major": 3.10, "ring_minor": 0.018, "ring_tilt_x": 69, "ring_tilt_z": -12, "near_sun": False, "stars_seed": 24},
    "anillos_gas_dorado": {"kind": "gas_ringed", "name": "Golden_Gas_Ringed_World", "gas_palette": "gold", "atmosphere": (1.0, 0.66, 0.22, 1), "atmosphere_strength": 0.10, "rings": True, "ring_color": (0.95, 0.72, 0.32, 0.33), "ring_alpha": 0.33, "ring_count": 9, "ring_major": 3.12, "ring_minor": 0.017, "ring_tilt_x": 65, "ring_tilt_z": 28, "near_sun": False, "stars_seed": 25},
    "anillos_desertico": {"kind": "ringed", "name": "Desert_Ringed_World", "material": {"color_a": (0.20, 0.11, 0.04, 1), "color_b": (0.80, 0.52, 0.20, 1), "color_c": (0.48, 0.30, 0.11, 1), "noise_scale": 9.5, "roughness": 0.93, "specular": 0.10}, "atmosphere": (0.92, 0.58, 0.24, 1), "atmosphere_strength": 0.06, "rings": True, "ring_color": (0.80, 0.62, 0.40, 0.35), "ring_alpha": 0.35, "ring_count": 6, "ring_major": 3.04, "ring_minor": 0.023, "ring_tilt_x": 67, "ring_tilt_z": 8, "near_sun": False, "stars_seed": 26},
    "anillos_helado": {"kind": "ringed", "name": "Ice_Ringed_World", "material": {"color_a": (0.36, 0.52, 0.70, 1), "color_b": (0.92, 0.98, 1.0, 1), "color_c": (0.62, 0.82, 1.0, 1), "noise_scale": 7.0, "roughness": 0.50, "specular": 0.38}, "atmosphere": (0.42, 0.72, 1.0, 1), "atmosphere_strength": 0.14, "rings": True, "ring_color": (0.78, 0.92, 1.0, 0.40), "ring_alpha": 0.40, "ring_count": 7, "ring_major": 3.08, "ring_minor": 0.020, "ring_tilt_x": 72, "ring_tilt_z": -25, "near_sun": False, "stars_seed": 27},
    "anillos_toxico": {"kind": "ringed", "name": "Toxic_Ringed_World", "material": {"color_a": (0.04, 0.12, 0.04, 1), "color_b": (0.42, 0.88, 0.16, 1), "color_c": (0.12, 0.38, 0.08, 1), "noise_scale": 6.5, "roughness": 0.74, "specular": 0.20}, "atmosphere": (0.44, 1.0, 0.18, 1), "atmosphere_strength": 0.11, "rings": True, "ring_color": (0.55, 1.0, 0.30, 0.30), "ring_alpha": 0.30, "ring_count": 5, "ring_major": 3.00, "ring_minor": 0.026, "ring_tilt_x": 64, "ring_tilt_z": 34, "near_sun": False, "stars_seed": 28},
    "anillos_oceanico": {"kind": "continental_ringed", "name": "Oceanic_Ringed_World", "continental_profile": "archipelago", "atmosphere": (0.10, 0.62, 1.0, 1), "atmosphere_strength": 0.15, "rings": True, "ring_color": (0.40, 0.85, 1.0, 0.32), "ring_alpha": 0.32, "ring_count": 6, "ring_major": 3.12, "ring_minor": 0.021, "ring_tilt_x": 70, "ring_tilt_z": 18, "near_sun": False, "stars_seed": 29}
}

# ==================================================
# VALIDAR PRESET
# ==================================================
if PLANET_PRESET not in PRESETS:
    available = "\n".join(sorted(PRESETS.keys()))
    raise ValueError(f"Preset no válido: {PLANET_PRESET}\n\nDisponibles:\n{available}")

profile = PRESETS[PLANET_PRESET]

# ==================================================
# CREAR MATERIAL SEGÚN PRESET
# ==================================================
if profile["kind"] == "lava":
    planet_mat = create_lava_material(profile["name"] + "_Material", profile.get("lava_palette", "controlled"))

elif profile["kind"] == "gas":
    planet_mat = create_gas_material(profile["name"] + "_Material", GAS_PALETTES[profile["gas_palette"]])

elif profile["kind"] == "gas_ringed":
    planet_mat = create_gas_material(profile["name"] + "_Material", GAS_PALETTES[profile["gas_palette"]])

elif profile["kind"] == "continental":
    planet_mat = create_continental_material(profile["name"] + "_Material", CONTINENTAL_PROFILES[profile["continental_profile"]])

elif profile["kind"] == "continental_ringed":
    planet_mat = create_continental_material(profile["name"] + "_Material", CONTINENTAL_PROFILES[profile["continental_profile"]])

elif profile["kind"] in ["ringed", "rocky"]:
    m = profile["material"]
    planet_mat = create_principled_material(
        profile["name"] + "_Material",
        color_a=m["color_a"],
        color_b=m["color_b"],
        color_c=m.get("color_c"),
        noise_scale=m.get("noise_scale", 7.0),
        noise_detail=m.get("noise_detail", 11.0),
        roughness=m.get("roughness", 0.80),
        specular=m.get("specular", 0.20),
        bump_strength=m.get("bump_strength", 0.065)
    )

else:
    raise ValueError("Tipo de preset no soportado.")

# ==================================================
# CREAR PLANETA
# ==================================================
planet = create_planet(profile["name"], planet_mat, radius=2.0)

if profile["kind"] in ["lava", "rocky", "ringed", "continental", "continental_ringed"]:
    add_displacement(
        planet,
        profile["name"] + "_Surface_Relief",
        kind='VORONOI',
        strength=profile.get("displacement_strength", 0.014),
        scale=1.1
    )

planet.rotation_euler = (
    math.radians(-7),
    0,
    math.radians(20)
)

# ==================================================
# CAPA DE NUBES PARA CONTINENTALES
# ==================================================
if profile["kind"] in ["continental", "continental_ringed"]:
    continental_profile = CONTINENTAL_PROFILES[profile["continental_profile"]]
    if continental_profile.get("clouds", False):
        add_cloud_layer(
            radius=2.038,
            alpha=continental_profile.get("cloud_alpha", 0.34),
            scale=continental_profile.get("cloud_scale", 7.0)
        )

# ==================================================
# ATMÓSFERA
# ==================================================
atmosphere_color = profile.get("atmosphere")

if atmosphere_color is None and profile["kind"] in ["continental", "continental_ringed"]:
    atmosphere_color = CONTINENTAL_PROFILES[profile["continental_profile"]].get("atmosphere")

if atmosphere_color is not None:
    strength = profile.get(
        "atmosphere_strength",
        CONTINENTAL_PROFILES.get(profile.get("continental_profile", ""), {}).get("atmosphere_strength", 0.12)
    )
    add_atmosphere(
        radius=2.12,
        color=atmosphere_color,
        strength=strength
    )

# ==================================================
# ANILLOS
# ==================================================
if profile.get("rings", False):
    add_rings(profile)

# ==================================================
# FONDO, SOL, LUCES Y CÁMARA
# ==================================================
add_starfield(
    seed=profile.get("stars_seed", 10),
    count=90 if not profile.get("near_sun") else 45
)

if profile.get("near_sun"):
    add_near_sun_hint()

setup_lighting(near_sun=profile.get("near_sun", False))
setup_camera()

# ==================================================
# SELECCIÓN FINAL
# ==================================================
bpy.ops.object.select_all(action='DESELECT')
planet.select_set(True)
bpy.context.view_layer.objects.active = planet

print(f"PLANETA CREADO CORRECTAMENTE: {PLANET_PRESET}")
print("Cambia PLANET_PRESET al principio del script para generar otro planeta.")
