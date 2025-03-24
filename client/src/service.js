import axios from 'axios';
const apiUrl = "http://localhost:5017/"; 
// axios.defaults.baseURL = "http://localhost:5017";

// הוספת interceptor שתופס את השגיאות
axios.interceptors.response.use(
  response => response,
  error => {
    console.error("API Error:", error);
    return Promise.reject(error);  // מחזיר את השגיאה כדי שנוכל לטפל בה בהמשך
  }
);

export default {
  getTasks: async () => {
    const result = await axios.get(`${apiUrl}`)    
    return result.data;
  },

  addTask: async(name)=>{
    console.log('addTask', name)
    //TODO
    try {
      const result = await axios.post(`${apiUrl}`, { name, isComplete: false });
      return result.data;
    } catch (error) {
      console.error("Error adding task:", error);
      return null;
    }
  },

  setCompleted: async(id, isComplete)=>{
    console.log('setCompleted', {id, isComplete})
    //TODO
    try {
      
      const result = await axios.put(`${apiUrl}${id}`, { isComplete });
      return result.data;
    } catch (error) {
      console.error("Error setting completion:", error);
      return null;
    }
  },

  deleteTask:async(id)=>{
    console.log(`Deleting task at: ${apiUrl}/${id}`);
    console.log('deleteTask')
    try {
      await axios.delete(`${apiUrl}${id}`);
      return { success: true };}
     catch (error) {
      console.error("Error deleting task:", error);
      return { success: false };
    }
  }
};
