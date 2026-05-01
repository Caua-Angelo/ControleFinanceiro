import { BrowserRouter as Router, Routes, Route } from "react-router-dom";
import Inicial from "./Pages/index.tsx";
import Categoria from "./Pages/Categoria.tsx";
import Transacao from "./Pages/Transacao.tsx";
import Layout from "./Components/layout.tsx";
import { LoginPage } from "./Pages/LoginPage.tsx";
import { PrivateRoute } from "./routes/PrivateRoute";

function App() {
  return (
    <Router>
      <Routes>
        <Route path="/login" element={<LoginPage />} />

        <Route
          path="/"
          element={
            <PrivateRoute>
              <Layout />
            </PrivateRoute>
          }
        >
          <Route index element={<Inicial />} />
          <Route path="categoria" element={<Categoria />} />
          <Route path="transacao" element={<Transacao />} />
        </Route>
      </Routes>
    </Router>
  );
}

export default App;
