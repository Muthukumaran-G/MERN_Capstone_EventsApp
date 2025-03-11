import axios from "axios";
import { useLoading } from "../context/LoadingContext";

const useAxiosInterceptor = () => {
  const { setLoading } = useLoading();

  axios.interceptors.request.use((config) => {
    setLoading(true);
    return config;
  });

  axios.interceptors.response.use(
    (response) => {
      setLoading(false);
      return response;
    },
    (error) => {
      setLoading(false);
      return Promise.reject(error);
    }
  );
};

export default useAxiosInterceptor;