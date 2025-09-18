import React, { useState } from "react";
import { Card, CardContent } from "@/components/ui/card";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function App() {
  const [symptoms, setSymptoms] = useState("");
  const [result, setResult] = useState(null);
  const [loading, setLoading] = useState(false);

  const handleCheckSymptoms = async () => {
    if (!symptoms) return;
    setLoading(true);
    try {
      const response = await fetch("http://localhost:5000/api/symptom/check", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ symptoms }),
      });
      const data = await response.json();
      setResult(data);
    } catch (error) {
      console.error(error);
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="min-h-screen bg-gray-100 flex items-center justify-center p-6">
      <Card className="w-full max-w-lg shadow-lg rounded-2xl">
        <CardContent className="p-6">
          <h1 className="text-2xl font-bold text-center mb-4">
            🩺 AI Healthcare Assistant
          </h1>

          <div className="space-y-3">
            <Input
              placeholder="Enter your symptoms..."
              value={symptoms}
              onChange={(e) => setSymptoms(e.target.value)}
              className="p-2 border rounded-xl"
            />

            <Button
              onClick={handleCheckSymptoms}
              className="w-full rounded-xl bg-blue-600 hover:bg-blue-700"
              disabled={loading}
            >
              {loading ? "Checking..." : "Check Symptoms"}
            </Button>
          </div>

          {result && (
            <div className="mt-6 bg-white p-4 rounded-xl shadow-md space-y-2">
              <h2 className="text-lg font-semibold">Results</h2>
              <p><strong>Possible Conditions:</strong> {result.possibleConditions?.join(", ")}</p>
              <p><strong>Advice:</strong> {result.advice}</p>
              <p><strong>Confidence:</strong> {Math.round(result.confidence * 100)}%</p>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
