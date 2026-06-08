# **FarmReach — Real‑Time Robotic Rehabilitation Platform**  
*A unified ROS2–Unity system for upper‑limb rehabilitation using a 2D planar robotic arm.*



## **About**
Real‑time rehabilitation platform integrating a 2D planar robotic arm, ROS2 processing pipeline, and an interactive Unity environment. The system provides low‑latency movement feedback, structured minigames, and persistent performance tracking to support home‑based upper‑limb therapy. Built as part of an individual MEng project, the platform demonstrates a complete closed‑loop architecture spanning embedded sensing, middleware communication, kinematics, analysis, and user‑facing interaction.

---

## **System Overview**
FarmReach consists of two coordinated subsystems, accessible via the following repositories:

- **[FarmReach‑Unity](https://github.com/el22mf/FarmReach-unity)** — real‑time rehabilitation environment with minigames, scoring, and user/session management
- **[FarmReach‑ROS2](https://github.com/el22mf/FarmReach-ros2)** — kinematics, performance metrics, data logging, and the ROS2 communication pipeline

Together, they form a closed‑loop rehabilitation system:

1. ESP32 reads joint angles  
2. Unity receives raw angles and forwards them to ROS2  
3. ROS2 computes end‑effector pose + metrics  
4. Unity visualises movement and runs rehabilitation tasks  
5. ROS2 logs results to SQLite for longitudinal tracking  

---

## **Architecture**
The platform integrates four layers:

### **1. Embedded Sensing (ESP32)**
- Reads joint angles + force data  
- Sends serial data to Unity at ~60 Hz  

### **2. ROS2 Processing**
- Forward kinematics  
- Task‑specific metric generation  
- SQLite‑based data persistence  
- ROS–Unity TCP bridge  

### **3. Unity Rehabilitation Environment**
- Low‑latency visualisation  
- Structured minigames (linear, rotational, endurance)  
- Unified scoring system  
- User login + session management  

### **4. Data Storage**
- Lightweight SQLite database  
- User accounts, session summaries, task‑level metrics  
- Supports longitudinal rehabilitation tracking  

---

## **Key Features**
- Real‑time closed‑loop control (~60 Hz)  
- Low‑latency ROS2–Unity communication  
- Forward kinematics for 2‑link planar arm  
- Structured rehabilitation tasks  
- Unified scoring + feedback  
- Persistent user data + session history  
- Scalable architecture for future clinical expansion  

---

## **Results Summary**
From the project evaluation:

- **Stable 60 Hz update rate** with no packet loss  
- **Consistent scoring** across task types  
- **Positive user feedback** (4.3/5 usability score)  
- **Robust communication pipeline** validated using a scaled prototype rig  

---

## **Future Work**
- Adaptive difficulty tuning  
- Expanded minigame library  
- Cloud‑based data storage  
- Clinical validation with stroke patients  
- 3D environment upgrade  

---

## **Author**
**Matthew Foster**  
MEng Mechatronics & Robotics Engineering  
University of Leeds  
